namespace Message.Domain.Entities;

public class User
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// 頭像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 密碼，重要操作需要輸入密碼
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 電子郵箱
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}