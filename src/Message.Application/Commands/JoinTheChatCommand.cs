using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
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
    
    public async Task<bool> HandleAsync(JoinTheChatCommand request)
    {
        // description - 
        //     設定 partner id
        if (!_setPartnerId(request.Connection, request.Uri))
            return false;

        // description - 
        //     設定聊天室 uri
        request.Connection.Uri = request.Uri;
        
        // description - 
        //     回應用戶
        await _replyRequestAsync(request.Connection.WebSocket, request.Uri);
        
        // description -
        //     表示設定完成
        return true;
    }


    private bool _setPartnerId(ConnectionModel connection, string uri)
    {
        if (!long.TryParse(uri.Split("/")[0], out var buyerId))
            return false;

        if (!long.TryParse(uri.Split("/")[1], out var itemId))
            return false;
        
        if (connection.UserId != buyerId)
        {
            connection.PartnerId = buyerId;
        }
        else
        {
            var item = _itemRepository.Get(itemId);
            connection.PartnerId = item.User.Id;
        }

        return true;
    }
        
    
    private async Task _replyRequestAsync(WebSocket socket, string uri) 
    {
        var msg = new MessageModel
        {
            Id = 0,
            Uri = uri,
            ReceiverId = 0,
            Content = "",
            Type = NoteType.join,
            Status = 0
        };
            
        var content = JsonSerializer.Serialize(msg, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        var body    = Encoding.UTF8.GetBytes(content);


        var buffer  = ArrayPool<byte>.Shared.Rent(body.Length);
        try
        {
            // processing - 將訊息放入剛剛租用的記憶體空間內
            body.CopyTo(buffer,0);
            var segment = new ArraySegment<byte>(buffer, 0, body.Length);
                
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(
                    segment, 
                    WebSocketMessageType.Text, 
                    endOfMessage: true,
                    CancellationToken.None);
            }
        }
        finally
        {
            // processing - 向系統歸還一塊記憶體空間
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}