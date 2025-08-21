using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

/// <summary>
/// 將要傳送出去的 message dto
/// </summary>
public struct MessageModel
{
    /// <summary>
    /// 訊息 ID
    /// </summary>
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    /// <summary>
    /// 聊天室 URI
    /// </summary>
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
    
    ///// <summary>
    ///// 收訊者
    ///// </summary>
    [JsonPropertyName("receiverId")]
    public required long ReceiverId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    [JsonPropertyName("type")]
    public required NoteType Type { get; set; }

    /// <summary>
    /// 狀態: 0 表示沒有異常
    /// </summary>
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    /// <summary>
    /// 標籤，主要是用來讓用戶查看訊息是否成空傳送出去用的
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
}

public static partial class MapExtension
{
    public static MessageModel ToModel(this Note note)
    {
        return new MessageModel()
        {
            Id = note.Id,
            Uri = $"{note.BuyerId}/{note.ItemId}",
            ReceiverId = note.ReceiverId,
            Content = note.Content,
            Type = note.Type,
            Status = 0
        };
    }
}
