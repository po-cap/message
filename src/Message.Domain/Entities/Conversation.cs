namespace Message.Domain.Entities;

public class Conversation
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 買家
    /// </summary>
    public User Buyer { get; set; }

    /// <summary>
    /// 賣家
    /// </summary>
    public User Seller { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Item Item { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public ICollection<Note> Notes { get; set; }
}