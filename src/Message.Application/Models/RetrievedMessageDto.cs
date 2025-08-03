using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

/// <summary>
/// 讀取到的 message
/// </summary>
public class RetrievedMessageDto
{
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
    /// <summary>
    /// 映射到傳送出去的 message dto
    /// </summary>
    /// <param name="msg">上傳上來的訊息</param>
    /// <param name="id">訊息的 ID</param>
    /// <param name="userId">傳訊者的 ID</param>
    /// <returns></returns>
    public static SubmittedMessageDto ToSubmittedMessage(this RetrievedMessageDto msg, long id, long userId)
    {
        return new SubmittedMessageDto()
        {
            ConversationId = msg.ConversationId,
            To = msg.To,
            Type = msg.Type,
            Content = msg.Content,
            Id = id,
            From = userId
        };
    }
}
