using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

public struct MessageDto
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    [JsonPropertyName("itemId")]
    public long ItemId { get; set; }
    
    /// <summary>
    /// 收訊者
    /// </summary>
    [JsonPropertyName("to")]
    public long To { get; set; }

    ///// <summary>
    ///// 發訊者
    ///// </summary>
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
}

public static partial class DtoExtension
{
    public static Note ToDomain(this MessageDto data, bool isRead = false)
    {
        var now = DateTimeOffset.Now;
        
        return new Note()
        {
            ItemId = data.ItemId,
            SenderId = data.From,
            ReceiverId = data.To,
            Type = data.Type,
            Content = data.Content,
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
        };
    }
}
