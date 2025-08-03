using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

/// <summary>
/// 將要傳送出去的 message dto
/// </summary>
public struct SubmittedMessageDto
{
    /// <summary>
    /// 建立成功回傳訊息
    /// </summary>
    /// <returns></returns>
    public static SubmittedMessageDto Success()
    {
        return new SubmittedMessageDto()
        {
            Type = DataType.success,
            Content = "",
            ConversationId = 0,
            To = 0,
            From = 0
        };
    }

    /// <summary>
    /// 訊息 ID
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    /// <summary>
    /// 商品 ID
    /// </summary>
    [JsonPropertyName("conversationId")]
    public long ConversationId { get; set; }
    
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
    public static SubmittedMessageDto ToSuccess(this SubmittedMessageDto data)
    {
        data.Type = DataType.success;
        return data;
    }
    
    
    public static Note ToDomain(this SubmittedMessageDto data, bool isRead = false)
    {
        var now = DateTimeOffset.Now;
        
        return new Note()
        {
            Id = data.Id,
            ConversationId = data.ConversationId,
            SenderId = data.From,
            ReceiverId = data.To,
            Type = data.Type,
            Content = data.Content,
            CreatedAt = now,
            ReadAt = isRead ? now : null,
        };
    }

    public static SubmittedMessageDto ToDto(this Note note)
    {
        return new SubmittedMessageDto()
        {
            Id = note.Id,
            To = note.ReceiverId,
            From = note.SenderId,
            Content = note.Content,
            Type = note.Type,
        };
    }
}
