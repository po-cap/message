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

internal class Messenger : IMessenger
{
    private readonly SnowflakeId _snowflake;
    private readonly ConcurrentDictionary<string, List<WebSocket>> _conversations;
    
    
    private readonly IServiceScopeFactory _scopeFactory;
    
    public Messenger(SnowflakeId snowflake, IServiceScopeFactory scopeFactory)
    {
        _conversations = [];
        _snowflake = snowflake;
        _scopeFactory = scopeFactory;
    }
    
    public async Task RunAsync(WebSocket socket, long userId, long buyerId, long itemId)
    {
        // processing -
        //     嘗試將 Socket 加入 Hash Table 中，若嘗試失敗退出 
        var success = false;
        for (var i = 0; i < 3; i++)
        {
            // 對話是否存在記憶體中
            var sockets = (
                from x in _conversations 
                where x.Key == $"{buyerId}/{itemId}" 
                select x.Value).FirstOrDefault();

            // 將 socket 加入對話中
            if (sockets == null)
            {
                success = _conversations.TryAdd($"{buyerId}/{itemId}", new List<WebSocket> { socket });
            }
            else
            {
                sockets.Add(socket);
                success = true;
            }
            
            // 查看 socket 是否成功加入對話中
            if (success) 
                break;
            
            Thread.Sleep(100);
        }

        if (success)
        {
            // 跑邏輯
            await _runAsync(
                socket: socket, 
                userId: userId, 
                buyerId: buyerId, 
                itemId: itemId);
        }
    }

    private async Task _runAsync(WebSocket socket, long userId, long buyerId, long itemId)
    {
        Item item;
        using (var scope = _scopeFactory.CreateScope())
        {
            var itemRepo = scope.ServiceProvider.GetService<IItemRepository>();
            if(itemRepo == null) throw new Exception();
            item = itemRepo.Get(itemId);
        }
        
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
                    
                    await _SendMessageAsync(
                        frame: frame,
                        userId: userId,
                        buyerId: buyerId, 
                        itemId: itemId,
                        sellerId: item.User.Id);
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
            _removeSocket(socket, buyerId, itemId);
        }
        
    }

    private void _removeSocket(WebSocket socket, long buyerId, long itemId)
    {
        var sockets = (
            from x in _conversations 
            where x.Key == $"{buyerId}/{itemId}" 
            select x.Value).FirstOrDefault();

        sockets?.Remove(socket);

        if (sockets?.Count == 0)
        {
            for (var i = 0; i < 3; i++)
            {
                if (_conversations.TryRemove($"{buyerId}/{itemId}", out var _))
                    break;
                Thread.Sleep(100);
            }
        }
    }
    
    /// <summary>
    /// 傳送訊息
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="userId"></param>
    /// <param name="buyerId"></param>
    /// <param name="itemId"></param>
    /// <param name="sellerId"></param>
    private async Task _SendMessageAsync(FrameModel frame, long userId, long buyerId, long itemId, long sellerId)
    {
        var messageId = _snowflake.Get();
        
        var sockets = (
            from x in _conversations 
            where x.Key == $"{buyerId}/{itemId}"
            select x.Value).FirstOrDefault();

        if (sockets is not null)
        {
            var message = frame.ToMessageModel(
                id: messageId,
                userId: userId,
                buyerId: buyerId);
            
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
            foreach (var socket in sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        segment, 
                        WebSocketMessageType.Text, 
                        endOfMessage: true,
                        CancellationToken.None);
                }
            }
            
            // processing - 向系統歸還一塊記憶體空間
            pool.Return(buffer);
        }
        
        // description - 儲存訊息
        using (var scope = _scopeFactory.CreateScope())
        {
            var note = frame.ToDomain(                
                id: messageId,
                userId: userId,
                buyerId: buyerId,
                itemId: itemId,
                sellerId: sellerId,
                isRead: sockets != null && sockets.Count > 1);
            var noteRepo = scope.ServiceProvider.GetService<INoteRepository>();
            noteRepo?.Add(note);
        }
    }
}