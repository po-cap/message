namespace Message.Application.Models;

/// <summary>
/// 對於聊天室的資訊做個統整資訊，包括
/// 1. 聊天室資訊
/// 2. 聊天室中的未讀訊息
/// </summary>
public class SummaryModel
{
    /// <summary>
    /// 聊天室資訊
    /// </summary>
    public ChatroomModel Chatroom { get; set; }

    /// <summary>
    /// 未讀訊息
    /// </summary>
    public IEnumerable<MessageModel> Messages { get; set; }
}