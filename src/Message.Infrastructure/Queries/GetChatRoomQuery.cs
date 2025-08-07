using Amazon.Runtime.Internal;
using Message.Application.Models;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetChatRoomQuery : IRequest<ChatroomModel>
{
    /// <summary>
    /// 買家
    /// </summary>
    public required long BuyerId { get; set; }

    /// <summary>
    /// 商品鏈結
    /// </summary>
    public required long ItemId { get; set; }

    /// <summary>
    /// 使用者
    /// </summary>
    public required long UserId { get; set; }
}

public class GetChatRoomHandler : IRequestHandler<GetChatRoomQuery,ChatroomModel>
{
    private readonly AppDbContext _context;

    public GetChatRoomHandler(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<ChatroomModel> HandleAsync(GetChatRoomQuery request)
    {
        // variables -
        //     unreadMessages: 此聊天室的未讀訊息
        //     item          : 商品鏈結
        //     buyer         : 買家
        //     isBuyer       : 是否是買家提出查詢
        var unreadMessages = _context.Notes
                                .Where(x => 
                                    x.BuyerId == request.BuyerId && 
                                    x.ItemId == request.ItemId && 
                                    x.ReadAt == null)
                                .OrderByDescending(x => x.CreatedAt)
                                .ToList();
        var item           = _context.Items.Include(x => x.User).First(x => x.Id == request.ItemId);
        var buyer          = _context.Users.First(x => x.Id == request.BuyerId);
        var isBuyer        = request.UserId == request.BuyerId;
        
        // processing - 
        var chatRoom = new ChatroomModel
        {
            Uri = $"{request.BuyerId}/{request.ItemId}",
            PartnerId = isBuyer ? item.User.Id : buyer.Id,
            Title = isBuyer ? item.User.DisplayName : buyer.DisplayName,
            Avatar = isBuyer ? item.User.Avatar : buyer.Avatar,
            Photo = item.Albums[0],
            UnreadCount = unreadMessages.Count,
            LastMessageType = unreadMessages.Last().Type,
            LastMessage = unreadMessages.Last().Content,
            UpdateAt = unreadMessages.Last().CreatedAt,
        };

        return Task.FromResult(chatRoom);
    }
}