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

public static partial class MapExtension
{
    
    public static MessageModel ToModel(this Note note)
    {
        return new MessageModel()
        {
            Id = note.Id,
            From = note.SenderId,
            Content = note.Content,
            Type = note.Type,
        };
    }
}
