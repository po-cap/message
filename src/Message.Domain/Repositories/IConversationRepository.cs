using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IConversationRepository
{
    /// <summary>
    /// 新建對話
    /// POST /conversation endpoint 會用到
    /// </summary>
    /// <param name="buyerId"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    Conversation Add(long buyerId, long itemId);
    
    /// <summary>
    /// 取得單一對話內容
    /// GET /conversation/{buyerId}/{itemId} endpoint 會用到
    /// </summary>
    /// <param name="buyerId"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    Conversation Get(long buyerId, long itemId);
    
    /// <summary>
    /// 取得跟使用者有關的所有對話
    /// GET /conversation/{userId}} endpoint 會用到
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IEnumerable<Conversation> Get(long userId);
}