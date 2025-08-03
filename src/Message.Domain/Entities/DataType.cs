namespace Message.Domain.Entities;

public enum DataType
{
    /// <summary>
    /// 傳送成功訊號
    /// </summary>
    success = 0,
    
    /// <summary>
    /// 文字訊息
    /// </summary>
    text = 1,
    
    /// <summary>
    /// 貼圖
    /// </summary>
    sticker = 2,
    
    /// <summary>
    /// 照片
    /// </summary>
    image = 3,
    
    /// <summary>
    /// 影片
    /// </summary>
    video = 4,
}