using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
using Po.Media;

namespace Message.Application;

internal class Messenger : IMessenger
{
    private readonly IMediaService _mediaService;
    private readonly ConcurrentDictionary<string, WebSocket> _sockets;
    
    public Messenger(IMediaService mediaService)
    {
        _mediaService = mediaService;
        _sockets = [];
    }
    
    public async Task RunAsync(WebSocket socket, string userId)
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
            await _runAsync(socket, userId);
        }
    }

    private async Task _runAsync(WebSocket socket, string userId)
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
                    await _sendAsync(message);
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
    
    private void _remove(string userId)
    {
        for (var i = 0; i < 3; i++)
        {
            if (_sockets.TryRemove(userId, out _)) 
                break;
            Thread.Sleep(100);
        }
    }
    
    private async Task _sendAsync(MessageDto message)
    {
        switch(message.Type)
        {
            case DataType.text:
            case DataType.sticker:
                await _sendMessageAsync(message);
                break;
            case DataType.image:
                await _sendImageAsync(message);
                break;
            case DataType.video:
                await _sendVideoAsync(message);
                break;
        }
        
    }
    
    private async Task _sendMessageAsync(MessageDto message)
    {
        var socket = (
            from s in _sockets 
            where s.Key == message.To 
            select s.Value).FirstOrDefault();
        
        if (socket is not null)
        {
            var content = JsonSerializer.Serialize(message, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var body    = Encoding.UTF8.GetBytes(content);

            var buffer  = ArrayPool<byte>.Shared.Rent(body.Length);
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
        
        // TODO: 這裡要把 Message 存到 Repository 裡
    }
    
    private async Task _sendImageAsync(MessageDto message)
    {
        var socket = (
            from s in _sockets 
            where s.Key == message.From 
            select s.Value).FirstOrDefault();
        if (socket == null) return; 
                
        var buffer = ArrayPool<byte>.Shared.Rent(5 * 1024 * 1024);

        try
        {
            var request = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (request.MessageType != WebSocketMessageType.Binary) return;

            using var stream = new MemoryStream();
            await stream.WriteAsync(buffer, 0, request.Count);
            var media = await _mediaService.UploadAsync(stream, new UploadOption()
            {
                Type = MediaType.image,
                Directory = "message",
                Name = $"tmp/{Guid.NewGuid()}.jpeg"
            });

            message.Content = media.Url;
            await _sendMessageAsync(message);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
    
    private async Task _sendVideoAsync(MessageDto message)
    {
        var socket = (
            from s in _sockets 
            where s.Key == message.From 
            select s.Value).FirstOrDefault();
        if (socket == null) return; 
                
        var buffer = ArrayPool<byte>.Shared.Rent(25 * 1024 * 1024);

        try
        {
            var request = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (request.MessageType != WebSocketMessageType.Binary) return;

            using var stream = new MemoryStream();
            await stream.WriteAsync(buffer, 0, request.Count);
            var media = await _mediaService.UploadAsync(stream, new UploadOption()
            {
                Type = MediaType.image,
                Directory = "message",
                Name = $"tmp/{Guid.NewGuid()}.jpeg"
            });

            message.Content = media.Url;
            await _sendMessageAsync(message);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
    
}