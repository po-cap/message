using System.Text.Json;

namespace Message.Domain.Entities;

public class Item
{
    /// <summary>
    /// 連接 ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 相簿
    /// </summary>
    public required List<string> Albums { get; set; }
    
    /// <summary>
    /// 規格
    /// </summary>
    public JsonDocument? Specs { get; set; }

    /// <summary>
    /// 賣家
    /// </summary>
    public User User { get; set; }
}