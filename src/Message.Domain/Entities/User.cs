namespace Message.Domain.Entities;

public class User
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 顯示名稱
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 頭像
    /// </summary>
    public string Avatar { get; set; }
}