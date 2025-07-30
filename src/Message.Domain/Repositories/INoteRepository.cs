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
    /// 取得 - 訊息
    /// </summary>
    /// <param name="to"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    IEnumerable<Note> Get(long to, long from);

    /// <summary>
    /// 統計 - 為接受過得訊息量，and，每個對話的最後一則訊息
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    IEnumerable<Note> Summary(long to);
}