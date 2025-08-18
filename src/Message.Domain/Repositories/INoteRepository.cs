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
    /// 設置使用者已讀
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="uri"></param>
    void SetRead(long userId, string uri);
}