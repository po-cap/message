using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Entities;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class ExitTheChatCommand : IRequest<bool>
{
    /// <summary>
    /// 使用者連結
    /// </summary>
    public required ConnectionModel Connection { get; set; }
}


public class ExitTheChatHandler : IRequestHandler<ExitTheChatCommand, bool>
{
    private readonly IConnection _connection;

    public ExitTheChatHandler(IConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<bool> HandleAsync(ExitTheChatCommand request)
    {
        request.Connection.Uri = null;
        request.Connection.PartnerId = null;
        await _replyRequestAsync(request.Connection.WebSocket);
        return false;
    }
    
    private async Task _replyRequestAsync(WebSocket socket) 
    {
        var msg = new MessageModel
        {
            Id = 0,
            Uri = "",
            ReceiverId = 0,
            Content = "",
            Type = NoteType.exit,
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
