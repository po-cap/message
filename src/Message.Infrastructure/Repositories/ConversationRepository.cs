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

    public Conversation Get(long buyerId, long itemId)
    {
        var k = _dbContext.Conversations
            .Include(x => x.Item)
            .ThenInclude(x => x.User)
            .Include(x => x.Buyer)
            .FirstOrDefault(x => x.BuyerId == buyerId && x.ItemId == itemId);

        if (k == null) throw Failure.NotFound();
        
        _dbContext.Entry(k)
            .Collection(x => x.Notes)
            .Query()
            .Where(x => x.ReadAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Load();
        
        return k;
    }

    public IEnumerable<Conversation> Get(long userId)
    {
        var unreadMessages = _dbContext.Notes.Where(x => x.ReceiverId == userId || x.SenderId == userId).ToList();

        return unreadMessages.GroupBy(x => new { x.BuyerId, x.ItemId }).Select((msgs) =>
        {
            var conversation = _dbContext.Conversations.Find(msgs.Key.BuyerId, msgs.Key.ItemId);
            
            if(conversation == null) throw Failure.BadRequest();
            
            _dbContext.Entry(conversation).Reference(x => x.Buyer).Load();
            _dbContext.Entry(conversation).Reference(x => x.Item).Load();
            
            return conversation;
        });
    }

    
    public Conversation Add(long buyerId, long itemId)
    {
        var conversation = _dbContext.Conversations.Find(buyerId, itemId);

        if (conversation == null)
        {
            var buyer = _dbContext.Users.Find(buyerId);
            if(buyer == null)
                throw Failure.Unauthorized();
            
            var item = _dbContext.Items.Include(x => x.User).FirstOrDefault(x => x.Id == itemId);
            if(item == null)
                throw Failure.BadRequest();

            conversation = Conversation.New(buyer, item);

            _dbContext.Add(conversation);

            _dbContext.SaveChanges();
        }
        else
        {
            _dbContext.Entry(conversation).Reference(x => x.Buyer).Load();
            _dbContext.Entry(conversation).Reference(x => x.Item).Load();
            _dbContext.Entry(conversation.Item).Reference(x => x.User).Load();
        }

        return conversation;
    }
}