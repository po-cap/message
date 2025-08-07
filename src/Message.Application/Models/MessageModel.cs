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
    public long Id { get; set; }
    
    ///// <summary>
    ///// 收訊者
    ///// </summary>
    [JsonPropertyName("receiverId")]
    public long ReceiverId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    [JsonPropertyName("type")]
    public NoteType Type { get; set; }

    /// <summary>
    /// 狀態: 0 表示沒有異常
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

public static partial class MapExtension
{
    public static MessageModel ToModel(this Note note)
    {
        return new MessageModel()
        {
            Id = note.Id,
            ReceiverId = note.ReceiverId,
            Content = note.Content,
            Type = note.Type,
        };
    }
}
