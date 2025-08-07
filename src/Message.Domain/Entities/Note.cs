namespace Message.Domain.Entities;

public class Note
{
    /// <summary>
    /// 訊息 ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 接收者 ID
    /// </summary>
    public long ReceiverId { get; set; }

    /// <summary>
    /// 訊息類型
    /// </summary>
    public NoteType Type { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 讀取時間
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }
    
    /// <summary>
    /// ForeignKey - 買家 ID
    /// </summary>
    public long BuyerId { get; set; }
    
    /// <summary>
    /// Navigation Property - 買家
    /// </summary>
    public User Buyer { get; set; }
    
    /// <summary>
    /// ForeignKey - 商品鏈結 ID
    /// </summary>
    public long ItemId { get; set; }
    
    /// <summary>
    /// Navigation Property - 商品鏈結
    /// </summary>
    public Item Item { get; set; }
}