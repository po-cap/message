using Message.Domain.Entities;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class StartConversationCommand : IRequest<Conversation>
{
    /// <summary>
    /// 買家 ID
    /// </summary>
    public long BuyerId { get; set; }

    /// <summary>
    /// 商品(鏈結) ID
    /// </summary>
    public long ItemId { get; set; }
}

public class StartConversationHandler : IRequestHandler<StartConversationCommand, Conversation>
{
    private readonly IConversationRepository _conversationRepository;

    public StartConversationHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public Task<Conversation> HandleAsync(StartConversationCommand request)
    {
        var conversation = _conversationRepository.Add(request.BuyerId, request.ItemId);
        return Task.FromResult(conversation);
    }
}