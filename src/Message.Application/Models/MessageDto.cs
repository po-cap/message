using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

public struct MessageDto
{
    /// <summary>
    /// 收訊者
    /// </summary>
    [JsonPropertyName("to")]
    public long To { get; set; }

    /// <summary>
    /// 發訊者
    /// </summary>
    [JsonPropertyName("from")]
    public long From { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    [JsonPropertyName("type")]
    public DataType Type { get; set; }

    /// <summary>
    /// 傳訊者頭像
    /// </summary>
    [JsonPropertyName("senderAvatar")]
    public string SenderAvatar { get; set; }

    /// <summary>
    /// 傳訊者名稱
    /// </summary>
    [JsonPropertyName("senderName")]
    public string SenderName { get; set; }
}

public static partial class DtoExtension
{
    public static Note ToDomain(this MessageDto data, bool isRead = false)
    {
        var now = DateTimeOffset.Now;
        
        return new Note()
        {
            SenderId = data.From,
            ReceiverId = data.To,
            Type = data.Type,
            Content = data.Content,
            SenderAvatar = data.SenderAvatar,
            SenderName = data.SenderName,
            CreatedAt = now,
            GetAt = isRead ? now : null,
            ReadAt = isRead ? now : null,
        };
    }

    public static MessageDto ToDto(this Note note)
    {
        return new MessageDto()
        {
            To = note.ReceiverId,
            From = note.SenderId,
            Content = note.Content,
            Type = note.Type,
            SenderAvatar = note.SenderAvatar,
            SenderName = note.SenderName
        };
    }
}
