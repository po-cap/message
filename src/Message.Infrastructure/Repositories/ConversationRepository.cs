using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Po.Api.Response;

namespace Message.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _dbContext;
    private readonly SnowflakeId _snowflakeId;

    public ConversationRepository(AppDbContext dbContext, SnowflakeId snowflakeId)
    {
        _dbContext = dbContext;
        _snowflakeId = snowflakeId;
    }

    public IEnumerable<object> Get(long userId)
    {
        var unReadMsgs = _dbContext.Notes
            .Where(x => x.ReceiverId == userId && x.ReadAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.ConversationId,
                x.Type,
                x.Content
            })
            .ToList();
        
        return unReadMsgs.GroupBy(x => x.ConversationId).Select((msgs) =>
        {
            var conv = _dbContext.Conversations
                .Include(x => x.Buyer)
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .First(x => x.Id == msgs.Key);
            return new
            {
                UnreadCount = msgs.Count(),
                Id = conv.Id,
                Item = conv.Item,
                Buyer = conv.Buyer,
                lastMessageType = msgs.Last().Type,
                lastMessage = msgs.Last().Content,
            };
        });
    }
    
    public Conversation Add(long buyerId, long itemId)
    {
        var buyer = _dbContext.Users.First(x => x.Id == buyerId);
        if(buyer == null)
            throw Failure.NotFound();
        
        var item = _dbContext.Items.Include(x => x.User).FirstOrDefault(x => x.Id == itemId);
        if(item == null)
            throw Failure.NotFound();
        
        var conversation = new Conversation()
        {
            Id = _snowflakeId.Get(),
            Buyer = buyer,
            Seller = item.User,
            Item = item
        };

        _dbContext.Add(conversation);

        _dbContext.SaveChanges();

        return conversation;
    }
}