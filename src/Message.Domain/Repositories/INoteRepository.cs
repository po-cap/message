using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface INoteRepository
{
    /// <summary>
    /// 新增 - 訊息
    /// </summary>
    /// <param name="note"></param>
    void Add(Note note);
    
    /// <summary>
    /// 設置成已讀
    /// </summary>
    /// <param name="ids"></param>
    void SetRead(IEnumerable<long> ids);
}