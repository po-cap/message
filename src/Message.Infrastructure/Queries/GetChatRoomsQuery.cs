using Message.Application.Models;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetChatRoomsQuery : IRequest<IEnumerable<ChatroomModel>>
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public required long UserId { get; set; }
}

public class GetChatRoomsHandler : IRequestHandler<GetChatRoomsQuery, IEnumerable<ChatroomModel>>
{
    private readonly AppDbContext _context;

    public GetChatRoomsHandler(AppDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<ChatroomModel>> HandleAsync(GetChatRoomsQuery request)
    {
        // processing - 
        var unreadMessages = _context.Notes
            .Where(x => x.ReceiverId == request.UserId && x.ReadAt == null)
            .ToList();

        // processing - 
        var itemIds = unreadMessages.Select(x => x.ItemId).Distinct();

        var items = _context.Items
            .Include(x => x.User)
            .Where(x => itemIds.Contains(x.Id))
            .ToList();

        // processing - 
        var buyerIds = unreadMessages.Select(x => x.BuyerId).Distinct();

        var buyers = _context.Users
            .Where(x => buyerIds.Contains(x.Id))
            .ToList();
            
        // processing - 
        var chatRooms = unreadMessages.GroupBy(x => new { x.BuyerId, x.ItemId }).Select((notes) =>
        {
            var isBuyer = request.UserId == notes.Key.BuyerId;
            var buyer = buyers.First(x => x.Id == notes.Key.BuyerId);
            var item = items.First(x => x.Id == notes.Key.ItemId);
            var lastNote = notes.Last();
            
            return new ChatroomModel
            {
                Uri = $"{buyer.Id}/{item.Id}",
                PartnerId = isBuyer ? item.User.Id : buyer.Id,
                Title = isBuyer ? item.User.DisplayName : buyer.DisplayName,
                Avatar = isBuyer ? item.User.Avatar : buyer.Avatar,
                Photo = item.Albums[0],
                UnreadCount = notes.Count(),
                LastMessageType = lastNote.Type,
                LastMessage = lastNote.Content,
                UpdateAt = lastNote.CreatedAt
            };
        });

        // return - 
        return Task.FromResult(chatRooms);
    }
}
