using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Po.Api.Response;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetRoomQuery : IRequest<MessageModel>
{
    public required ConnectionModel Connection { get; set; }

    public required string Uri { get; set; }
}

public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, MessageModel>
{
    private readonly AppDbContext _context;

    public GetRoomQueryHandler(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<MessageModel> HandleAsync(GetRoomQuery request)
    {
        // 
        var (buyerId, itemId) = _uri(request.Uri);

        //
        var isBuyer = request.Connection.UserId == buyerId;
        var user = _context.Users.Find(buyerId);
        if (user == null) throw Failure.BadRequest();
        var item = _context.Items.Find(itemId);
        if (item == null) throw Failure.BadRequest();


        var room = new RoomModel
        {
            Uri = request.Uri,
            PartnerId = isBuyer ? item!.User.Id : user!.Id,
            Title = isBuyer ? item.User.DisplayName : user.DisplayName,
            Avatar = isBuyer ? item.User.Avatar : user.Avatar,
            Photo = item.Albums[0]
        };
        
        var content = JsonSerializer.Serialize(
            room,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        return Task.FromResult(new MessageModel
        {
            Id = 0,
            Uri = "",
            ReceiverId = request.Connection.UserId,
            Content = content,
            Type = NoteType.chatroom,
            Status = 0
        });
    }
    
    /// <summary>
    /// 解讀 uri
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private (long buyerId, long itemId) _uri(string uri)
    {
        var elements = uri.Split("/");

        if(!long.TryParse(elements[0], out var buyerId))
            throw Failure.BadRequest(title: "無法正確解讀買家");
        
        if(!long.TryParse(elements[1], out var itemId))
            throw Failure.BadRequest(title: "無法正確解讀商品鏈結");
        
        return (buyerId, itemId);
    }
}