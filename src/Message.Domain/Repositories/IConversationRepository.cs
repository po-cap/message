using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IConversationRepository
{
    Conversation Add(long buyerId, long itemId);
    
    IEnumerable<object> Get(long userId);
}