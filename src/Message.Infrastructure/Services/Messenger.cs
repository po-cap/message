using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Commands;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Entities;
using Message.Infrastructure.Queries;
using Microsoft.Extensions.Logging;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Services;

internal class Messenger : IMessenger
{
    private readonly ILogger<Messenger> _logger;
    private readonly IConnection _connection;
    private readonly IMediator _mediator;
    
    public Messenger(ILogger<Messenger> logger, IConnection connection, IMediator mediator)
    {
        _logger = logger;
        _connection = connection;
        _mediator = mediator;
    }

    public async Task RunAsync(WebSocket socket, long userId)
    {
        // --------------------------------------------------------------------------------
        // 代表用戶連接的 object
        // --------------------------------------------------------------------------------
        var connection = new ConnectionModel()
        {
            UserId = userId,
            WebSocket = socket
        };
        
        // --------------------------------------------------------------------------------
        //     嘗試將 Socket 加入 Hash Table 中，若嘗試失敗退出 
        // --------------------------------------------------------------------------------
        bool success = false;
        for (var i = 0; i < 3; i++)
        {
            if (_connection.Users.TryAdd(userId, connection))
            {
                _logger.LogDebug($"{userId} 連線成功");
                success = true;
                break;
            }

            Thread.Sleep(100);
        }

        if (!success)
        {
            _logger.LogDebug($"{userId} 連線失敗");
            await socket.CloseAsync(
                WebSocketCloseStatus.InternalServerError,
                "",
                CancellationToken.None);
        }
        
        // --------------------------------------------------------------------------------
        // 跑回圈
        // --------------------------------------------------------------------------------
        var buffer = ArrayPool<byte>.Shared.Rent(1024 * 4);

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                // processing - 解收 socket 傳來的訊息
                var request = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                // condition - 如果是文字訊息，就處理，並發送給其他使用者
                if (request.MessageType == WebSocketMessageType.Text)
                {
                    var content = Encoding.UTF8.GetString(buffer, 0, request.Count);

                    var frame = JsonSerializer.Deserialize<FrameModel>(content);
                    if (frame == null)
                        continue;

                    MessageModel message;
                    switch (frame.Type)
                    {
                        case NoteType.ping:
                            await _mediator.SendAsync(new PingCommand()
                            {
                                Connection = connection
                            });
                            break;
                        case NoteType.text:
                        case NoteType.sticker:
                        case NoteType.image:
                        case NoteType.video:
                            message = await _mediator.SendAsync(new SendMessageCommand
                            {
                                Connection = connection,
                                Type = frame.Type,
                                Content = frame.Content
                            });
                            await _replyRequestAsync(connection.WebSocket, message);
                            break;
                        case NoteType.join:
                            await _mediator.SendAsync(new JoinTheChatCommand
                            {
                                Connection = connection,
                                Uri = frame.Content
                            });
                            break;
                        case NoteType.exit:
                            await _mediator.SendAsync(new ExitTheChatCommand()
                            {
                                Connection = connection
                            });
                            break;
                        case NoteType.read:
                            await _mediator.SendAsync(new SetReadCommand()
                            {
                                Ids = JsonSerializer.Deserialize<List<long>>(frame.Content) ?? []
                            });
                            break;
                        case NoteType.unread_count:
                            message = await _mediator.SendAsync(new GetUnreadCountQuery()
                            {
                                Connection = connection
                            });
                            await _replyRequestAsync(connection.WebSocket, message);
                            break;
                        case NoteType.unread_messages:
                            message = await _mediator.SendAsync(new GetAllUnreadNotesQuery()
                            {
                                Connection = connection
                            });
                            await _replyRequestAsync(connection.WebSocket, message);
                            break;   
                        case NoteType.chatroom:
                            message = await _mediator.SendAsync(new GetRoomQuery
                            {
                                Connection = connection,
                                Uri = frame.Content
                            });
                            await _replyRequestAsync(connection.WebSocket, message);
                            break;  
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
                // condition - 如果是 close 幀，就關閉 web socket
                else if (request.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(
                        request.CloseStatus!.Value,
                        request.CloseStatusDescription,
                        CancellationToken.None);
                }
            }
        }
        catch (Exception _)
        {
            await socket.CloseAsync(
                WebSocketCloseStatus.InternalServerError,
                "",
                CancellationToken.None);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);

            for (int i = 0; i < 3; i++)
            {
                if (_connection.Users.TryRemove(userId, out var _))
                    break;
                else
                    Thread.Sleep(100);
            }
        }  
    }
    
    
    /// <summary>
    /// 回應使用者
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="message"></param>
    private async Task _replyRequestAsync(WebSocket socket, MessageModel message) 
    {
        var content = JsonSerializer.Serialize(message, new JsonSerializerOptions()
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