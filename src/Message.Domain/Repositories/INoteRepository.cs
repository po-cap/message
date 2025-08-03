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
    /// 取得 - 未讀訊息
    /// </summary>
    /// <param name="receiverId">收訊者 ID</param>
    /// <param name="conversationId">對話 ID</param>
    /// <returns></returns>
    IEnumerable<Note> Get(long receiverId, long conversationId);
}