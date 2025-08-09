using Message.Application.Models;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class JoinTheChatCommand : IRequest<bool>
{
    /// <summary>
    /// 使用者連接
    /// </summary>
    public required ConnectionModel Connection { get; set; }
    
    /// <summary>
    /// 聊天室 uri
    /// </summary>
    public required string Uri { get; set; }
}

public class JoinTheChatHandler : IRequestHandler<JoinTheChatCommand, bool>
{
    private readonly IItemRepository _itemRepository;

    public JoinTheChatHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }
    
    public Task<bool> HandleAsync(JoinTheChatCommand request)
    {
        // description - 
        //     設定 partner id
        if (!long.TryParse(request.Uri.Split("/")[0], out var buyerId))
            return Task.FromResult(false);

        if (!long.TryParse(request.Uri.Split("/")[1], out var itemId))
            return Task.FromResult(false);
        
        if (request.Connection.UserId != buyerId)
        {
            request.Connection.PartnerId = buyerId;
        }
        else
        {
            var item = _itemRepository.Get(itemId);
            request.Connection.PartnerId = item.User.Id;
        }

        // description - 
        //     設定聊天室 uri
        request.Connection.Uri = request.Uri;
        
        // description -
        //     表示設定完成
        return Task.FromResult(true);
    }
}