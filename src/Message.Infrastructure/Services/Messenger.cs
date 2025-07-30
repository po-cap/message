using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Message.Infrastructure.Services;

/// <summary>
/// 代表一個使用者的長連接
/// </summary>
public class Connection
{
    public static Connection New(WebSocket socket, User user)
    {
        return new Connection()
        {
            Socket = socket,
            User = user,
        };
    }
    
    /// <summary>
    /// 長連接
    /// </summary>
    public required WebSocket Socket { get; set; }

    /// <summary>
    /// 使用者
    /// </summary>
    public required User User { get; set; }
}


internal class Messenger : IMessenger
{
    //private readonly ConcurrentDictionary<long, WebSocket> _sockets;
    private readonly ConcurrentDictionary<long, Connection> _sockets;
    
    
    private readonly IServiceScopeFactory _scopeFactory;
    
    public Messenger(
        IServiceScopeFactory scopeFactory)
    {
        _sockets = [];
        
        _scopeFactory = scopeFactory;
    }
    
    //public async Task RunAsync(WebSocket socket, string userId)
    //{
    //    if(!long.TryParse(userId, out var id))
    //        throw Failure.Unauthorized();
    //    
    //    // processing -
    //    //     嘗試將 Socket 加入 Hash Table 中，若嘗試失敗退出 
    //    var success = false;
    //    for (var i = 0; i < 3; i++)
    //    {
    //        success = _sockets.TryAdd(id, socket);
    //        if (success) 
    //            break;
    //        Thread.Sleep(100);
    //    }
    //
    //    if (success)
    //    {
    //        await _runAsync(socket, id);
    //    }
    //}

    public async Task RunAsync(WebSocket socket, string token)
    {
        User user;
        using (var scope = _scopeFactory.CreateScope())
        {
            var userRepo = scope.ServiceProvider.GetService<IUserRepository>() ?? throw new Exception();
            user = await userRepo.GetAsync(token);
        }
        
        // processing -
        //     嘗試將 Socket 加入 Hash Table 中，若嘗試失敗退出 
        var success = false;
        for (var i = 0; i < 3; i++)
        {
            success = _sockets.TryAdd(user.Id, Connection.New(socket, user));
            if (success) 
                break;
            Thread.Sleep(100);
        }

        if (success)
        {
            await _runAsync(socket, user.Id);
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

                    var user = _sockets[userId].User;
                    message.From = user.Id;
                    message.SenderAvatar = user.Avatar;
                    message.SenderName = user.DisplayName;
                    
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
        var connection = (
            from x in _sockets 
            where x.Key == message.To 
            select x.Value).FirstOrDefault();

        if (connection is not null)
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
            if (connection.Socket.State == WebSocketState.Open)
            {
                await connection.Socket.SendAsync(
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
            var noteRepo = scope.ServiceProvider.GetService<INoteRepository>();
            noteRepo?.Add(message.ToDomain(isRead: connection is not null));
        }
    }
}