using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class PingCommand : IRequest<bool>
{
    /// <summary>
    /// 發出 ping 的用戶
    /// </summary>
    public required ConnectionModel Connection { get; set; }
}


public class PingHandler : IRequestHandler<PingCommand, bool>
{
    
    public async Task<bool> HandleAsync(PingCommand request)
    {
        var socket = request.Connection.WebSocket;
        
        var msg = new MessageModel
        {
            Id = 0,
            Uri = "",
            ReceiverId = 0,
            Content = "",
            Type = NoteType.ping,
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
            body.CopyTo(buffer, 0);
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
        catch (Exception _)
        {
            return false;
        }
        finally
        {
            // processing - 向系統歸還一塊記憶體空間
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return true;
    }
}