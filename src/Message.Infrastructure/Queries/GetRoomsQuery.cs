using Message.Application.Models;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Po.Api.Response;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetRoomsQuery : IRequest<IEnumerable<RoomModel>>
{
    /// <summary>
    /// 所有聊天室的 uri
    /// </summary>
    public required List<string> Uris { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    public required long UserId { get; set; }
}


public class GetRoomsHandler : IRequestHandler<GetRoomsQuery, IEnumerable<RoomModel>>
{
    private readonly AppDbContext _context;

    public GetRoomsHandler(AppDbContext context)
    {
        _context = context;
    }


    public Task<IEnumerable<RoomModel>> HandleAsync(GetRoomsQuery request)
    {
        //
        List<long> buyerIds = [];
        List<long> itemIds  = [];
        List<Item> items;
        List<User> users;
        List<RoomModel> rooms = [];

        // 
        foreach (var uri in request.Uris)
        {
            var (buyerId, itemId) = _uri(uri);
            buyerIds.Add(buyerId);
            itemIds.Add(itemId);
        }

        //
        users = _context.Users.Where(x => buyerIds.Contains(x.Id)).ToList();
        
        //
        items = _context.Items.Include(x => x.User).Where(x => itemIds.Contains(x.Id)).ToList();

        //
        for (var i = 0; i < request.Uris.Count; i++)
        {
            var isBuyer = request.UserId == users[i].Id;
            
            rooms.Add(new RoomModel()
            {
                Uri = request.Uris[i],
                PartnerId = isBuyer ? items[i].User.Id : users[i].Id,
                Title = isBuyer ? items[i].User.DisplayName : users[i].DisplayName,
                Avatar = isBuyer ? items[i].User.Avatar : users[i].Avatar,
                Photo = items[i].Albums[0],
            });
        }

        return Task.FromResult<IEnumerable<RoomModel>>(rooms);
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