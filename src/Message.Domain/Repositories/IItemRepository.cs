using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IItemRepository
{
    /// <summary>
    /// 取得商品資訊
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    List<Item> Get(long[] ids);
}