using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Message.Application.Models;

namespace Message.Presentation.Messages;

/// <summary>
/// Chat Room Mediator Interface
/// </summary>
public interface IChatRoom
{
    /// <summary>
    /// 使用者 - 建立長連接 (Web Socket)
    /// </summary>
    /// <param name="socket">連連接</param>
    /// <param name="userId">使用者 ID</param>
    void Register(WebSocket socket, string userId);
    
    /// <summary>
    /// 傳送訊息
    /// </summary>
    /// <param name="message">訊息</param>
    /// <returns></returns>
    Task SendAsync(MessageModel message);
    
    /// <summary>
    /// 使用者 - 斷開長連接 (Web Socket)
    /// </summary>
    /// <param name="socket">長連接</param>
    /// <param name="userId">使用者 ID</param>
    void UnRegister(WebSocket socket, string userId);
}

public class ChatRoom : IChatRoom
{
    private readonly Dictionary<string, WebSocket> _sockets;

    public ChatRoom()
    {
        _sockets = [];
    }
    
    public void Register(WebSocket socket, string userId)
    {
        _sockets.Add(userId, socket);
    }

    public async Task SendAsync(MessageModel message)
    {
        //// TODO: 判斷 Message 的種類，如果是 Image 或是 Video，那先將檔案上傳到 S3，再把 Attachment 的部分修改成 url
        //
        //var content = JsonSerializer.Serialize(message);
        //var body    = Encoding.UTF8.GetBytes(content);
        //var segment = new ArraySegment<byte>(body);
        //var socket = (
        //    from s in _sockets 
        //    where s.Key == message.To 
        //    select s.Value).FirstOrDefault();
        //
        //if (socket != null)
        //{
        //    await socket.SendAsync(
        //        segment, 
        //        WebSocketMessageType.Text, 
        //        true, 
        //        CancellationToken.None);
        //}
        //
        // TODO: 將 Message 存入資料庫
    }

    public void UnRegister(WebSocket socket, string userId)
    {
        _sockets.Remove(userId);
    }

    private ArraySegment<byte> ToBytes(MessageModel message)
    {
        var content = JsonSerializer.Serialize(message);
        var body    = Encoding.UTF8.GetBytes(content);
        return new ArraySegment<byte>(body);
    }
}

internal static class BytesExt
{
    internal static ArraySegment<byte> ToBytes(this MessageModel message)
    {
        var content = JsonSerializer.Serialize(message);
        var body    = Encoding.UTF8.GetBytes(content);
        return new ArraySegment<byte>(body);
    }

    internal static MessageModel? ToMessage(this WebSocketReceiveResult request)
    {
        // processing - 租借 4kb 空間
        var buffer = ArrayPool<byte>.Shared.Rent(1024 * 4);
        
        var content = Encoding.UTF8.GetString(buffer, 0, request.Count);
        var message = JsonSerializer.Deserialize<MessageModel>(content);
        
        // processing - 歸還 4kb 空間
        ArrayPool<byte>.Shared.Return(buffer);

        // return - 訊息
        return message;
    }
}


