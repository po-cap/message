using Message.Domain.Entities;

namespace Message.Application.Models;

public class ConversationDto
{
    
    /// <summary>
    /// 幾則未得
    /// </summary>
    public int UnreadCount { get; set; }
    
    /// <summary>
    /// 最後一則訊息
    /// </summary>
    public SubmittedMessageDto LastSubmittedMessage { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Item Item { get; set; }
}