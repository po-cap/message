namespace Message.Application.Models;

public class RoomModel
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
}