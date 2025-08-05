using Message.Domain.Entities;

namespace Message.Application.Models;

public class ConversationModel
{
    /// <summary>
    /// 取得 Conversation 資訊的 Uri
    /// </summary>
    public string Uri { get; set; }
    
    /// <summary>
    /// 對話對象
    /// </summary>
    public User Buyer { get; set; }

    /// <summary>
    /// 幾則未得
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 商品鏈結
    /// </summary>
    public Item Item { get; set; }

    /// <summary>
    /// 最後一筆訊息的類型
    /// </summary>
    public DataType? LastMessageType { get; set; }

    /// <summary>
    /// 最後一筆信息的內容
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// 用戶是否是買家
    /// </summary>
    public bool IsBuyer { get; set; }
}

public static partial class MapExtension
{
    public static ConversationModel ToModel(this Conversation entity, long userId)
    {
        var notes = entity.Notes.Where(x => x.ReadAt == null).OrderByDescending(x => x.CreatedAt);
        var lastNote = notes.LastOrDefault();

        return new ConversationModel()
        {
            Uri = $"/conversation/{entity.Buyer.Id}/{entity.Item.Id}",
            Buyer = entity.Buyer,
            UnreadCount = notes.Count(),
            Item = entity.Item,
            LastMessageType = lastNote?.Type,
            LastMessage = lastNote?.Content,
            IsBuyer = entity.Buyer.Id == userId
        };
        
        throw new NotImplementedException();
        //var lastMessage = entity.Notes.MinBy(x => x.CreatedAt);
        //
        //return new ConversationDto()
        //{
        //    Id = entity.Id,
        //    Item = entity.Item,
        //    Buyer = entity.Buyer,
        //    UnreadCount = entity.Notes.Count(x => x.ReadAt == null),
        //    LastMessageType = lastMessage?.Type,
        //    LastMessage = lastMessage?.Content
        //};
    }
} 
