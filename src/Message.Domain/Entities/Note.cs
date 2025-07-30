namespace Message.Domain.Entities;

public class Note
{
    /// <summary>
    /// 訊息 ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 發送者 ID
    /// </summary>
    public long SenderId { get; set; }

    /// <summary>
    /// 接收者 ID
    /// </summary>
    public long ReceiverId { get; set; }

    /// <summary>
    /// 訊息類型
    /// </summary>
    public DataType Type { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 發送者頭像
    /// </summary>
    public string SenderAvatar { get; set; }

    /// <summary>
    /// 發送者暱稱
    /// </summary>
    public string SenderName { get; set; }
    
    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 獲取時間（計算有幾條未讀訊息用的）
    /// </summary>
    public DateTimeOffset? GetAt { get; set; }
    
    /// <summary>
    /// 讀取時間
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }
}