using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="token">Json Web Token</param>
    /// <returns></returns>
    Task<User> GetAsync(string token);
}