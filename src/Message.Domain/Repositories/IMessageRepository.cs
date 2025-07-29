namespace Message.Domain.Repositories;
using Message = Message.Domain.Entities.Message;

public interface IMessageRepository
{
    /// <summary>
    /// 新增 - 訊息
    /// </summary>
    /// <param name="message"></param>
    void Add(Message message);
    
}