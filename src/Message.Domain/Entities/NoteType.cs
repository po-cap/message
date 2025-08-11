namespace Message.Domain.Entities;

public enum NoteType
{
    /// <summary>
    /// Ping Pong
    /// </summary>
    ping = 0,
    
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
    
    /// <summary>
    /// 進入聊天室
    /// </summary>
    join = 5,
    
    /// <summary>
    /// 退出聊天室
    /// </summary>
    exit = 6,
    
    /// <summary>
    /// 通知服務器訊息已讀過
    /// </summary>
    read = 7,
    
    /// <summary>
    /// 取得未讀訊息量
    /// </summary>
    unread_count = 8,
    
    /// <summary>
    /// 取得未讀訊息
    /// </summary>
    unread_messages = 9
}