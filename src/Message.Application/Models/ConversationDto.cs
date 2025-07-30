namespace Message.Application.Models;

public class ConversationDto
{
    /// <summary>
    /// 發送者 ID
    /// </summary>
    public long SenderId { get; set; }
    
    /// <summary>
    /// 幾則未得
    /// </summary>
    public int UnReadCount { get; set; }
    
    /// <summary>
    /// 最後一則訊息
    /// </summary>
    public MessageDto LastMessage { get; set; }
    
    /// <summary>
    /// 發送者頭像
    /// </summary>
    public string SenderAvatar { get; set; }
    
    /// <summary>
    /// 發送者暱稱
    /// </summary>
    public string SenderName { get; set; }
}