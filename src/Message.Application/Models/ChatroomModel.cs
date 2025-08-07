using Message.Domain.Entities;

namespace Message.Application.Models;

public class ChatroomModel
{
    /// <summary>
    /// 取得 Conversation 資訊的 Uri
    /// </summary>
    public required string Uri { get; set; }

    /// <summary>
    /// 聊天對象 ID
    /// </summary>
    public required long PartnerId { get; set; }

    /// <summary>
    /// 聊天室標題
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// 聊天室頭像
    /// </summary>
    public required string Avatar { get; set; }

    /// <summary>
    /// 聊天室商品照片
    /// </summary>
    public required string? Photo { get; set; }

    /// <summary>
    /// 未讀訊息數量
    /// </summary>
    public required int UnreadCount { get; set; }

    /// <summary>
    /// 最後一則訊息類型
    /// </summary>
    public required NoteType? LastMessageType { get; set; }

    /// <summary>
    /// 最後一則訊息
    /// </summary>
    public required string? LastMessage { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public required DateTimeOffset? UpdateAt { get; set; }
}