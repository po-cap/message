namespace Message.Domain.Entities;

public class Conversation
{
    public static Conversation New(User buyer, Item item)
    {
        return new Conversation()
        {
            Buyer = buyer,
            Item = item,
            BuyerId = buyer.Id,
            ItemId = item.Id,
        };
    }
    
    
    /// <summary>
    /// 買家
    /// </summary>
    public User Buyer { get; set; }
    
    /// <summary>
    /// 商品鏈結
    /// </summary>
    public Item Item { get; set; }
    
    /// <summary>
    /// Foreign Key 買家  
    /// </summary>
    public long BuyerId { get; set; }
    
    /// <summary>
    /// Foreign Key 商品鏈結
    /// </summary>
    public long ItemId { get; set; }
    
    /// <summary>
    /// 訊息
    /// </summary>
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}