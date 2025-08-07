using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

/// <summary>
/// 讀取到的 message
/// </summary>
public class FrameModel
{
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
}


public static partial class MapExtension
{
    /// <summary>
    /// 映射到傳送出去的 message dto
    /// </summary>
    /// <param name="msg">上傳上來的訊息</param>
    /// <param name="id">訊息的 ID</param>
    /// <param name="userId">傳訊者的 ID</param>
    /// <param name="buyerId">買家 ID</param>
    /// <param name="sellerId">賣家 ID</param>
    /// <param name="itemId">商品鏈結 ID</param>
    /// <returns></returns>
    public static MessageModel ToMessageModel(
        this FrameModel msg, 
        long id, 
        long userId,
        long buyerId,
        long sellerId,
        long itemId)
    {
        var isBuyer = userId == buyerId;
        
        
        return new MessageModel()
        {           
            Id = id,
            Uri = $"{buyerId}/{itemId}",
            ReceiverId = isBuyer ? sellerId : buyerId,
            Type = msg.Type,
            Content = msg.Content,
            Status = 0
        };
    }
    
    public static Note ToDomain(
        this FrameModel data,
        long id,
        long userId,
        long buyerId,
        long itemId,
        long sellerId,
        bool isRead = false)
    {
        var now = DateTimeOffset.Now;
        var isBuyer = userId == buyerId;
        
        return new Note()
        {
            Id = id,
            BuyerId = buyerId,
            ItemId = itemId,
            
            ReceiverId = isBuyer ? sellerId : buyerId,
            
            Type = data.Type,
            Content = data.Content,
            CreatedAt = now,
            ReadAt = isRead ? now : null,
        };
    }
}
