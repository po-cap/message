using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Message.Infrastructure.Services;

internal class Messenger : IMessenger
{
    //private readonly ConcurrentDictionary<long, WebSocket> _sockets;

    private readonly ILogger<Messenger> _logger;
    private readonly ConcurrentDictionary<long, WebSocket> _sockets;
    
    
    private readonly IServiceScopeFactory _scopeFactory;
    
    public Messenger(
        ILogger<Messenger> logger,
        IServiceScopeFactory scopeFactory)
    {
        _sockets = [];
        
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task RunAsync(WebSocket socket, long userId)
    {
        // processing -
        //     嘗試將 Socket 加入 Hash Table 中，若嘗試失敗退出 
        var success = false;
        for (var i = 0; i < 3; i++)
        {
            success = _sockets.TryAdd(userId, socket);
            if (success) 
                break;
            Thread.Sleep(100);
        }

        if (success)
        {
            // 跑邏輯
            await _runAsync(socket, userId);
        }
    }

    private async Task _runAsync(WebSocket socket, long userId)
    {
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
                    var message = JsonSerializer.Deserialize<MessageDto>(content);
                    message.From = userId;
                    
                    _logger.LogInformation($"收到信息：{content}");
                    
                    await _sendMessageAsync(message);
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
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            _remove(userId);
        }
    }
    
    private void _remove(long userId)
    {
        for (var i = 0; i < 3; i++)
        {
            if (_sockets.TryRemove(userId, out _)) 
                break;
            Thread.Sleep(100);
        }
    }
    
    
    private async Task _sendMessageAsync(MessageDto message)
    {
        var socket = (
            from x in _sockets 
            where x.Key == message.To 
            select x.Value).FirstOrDefault();

        if (socket is not null)
        {
            var content = JsonSerializer.Serialize(message, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var body    = Encoding.UTF8.GetBytes(content);

            // processing - 向系統租用一塊記憶體空間
            var pool = ArrayPool<byte>.Shared;
            var buffer  = pool.Rent(body.Length);
            
            // processing - 將訊息放入剛剛租用的記憶體空間內
            body.CopyTo(buffer,0);
            var segment = new ArraySegment<byte>(buffer, 0, body.Length);
            
            // processing - 發送消息
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(
                    segment, 
                    WebSocketMessageType.Text, 
                    endOfMessage: true,
                    CancellationToken.None);
            }
            
            // processing - 向系統歸還一塊記憶體空間
            pool.Return(buffer);
        }
        
        // description - 儲存訊息
        using (var scope = _scopeFactory.CreateScope())
        {
            var note = message.ToDomain(isRead: socket is not null);
            var noteRepo = scope.ServiceProvider.GetService<INoteRepository>();
            noteRepo?.Add(note);
        }
    }
}