using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    List<User> Get(long[] ids);
}