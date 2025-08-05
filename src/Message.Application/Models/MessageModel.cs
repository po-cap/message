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
    
    /// <summary>
    /// 買家 ID
    /// </summary>
    [JsonPropertyName("buyerId")]
    public long BuyerId { get; set; }

    /// <summary>
    /// 商品鏈結 ID
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

public static partial class MapExtension
{
    
    public static MessageModel ToModel(this Note note)
    {
        return new MessageModel()
        {
            Id = note.Id,
            BuyerId = note.BuyerId,
            ItemId = note.ItemId,
            
            To = note.ReceiverId,
            From = note.SenderId,
            
            Content = note.Content,
            Type = note.Type,
        };
    }
}
