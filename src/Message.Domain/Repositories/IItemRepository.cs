using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IItemRepository
{
    /// <summary>
    /// 取得商品資訊
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Item Get(long id);
}