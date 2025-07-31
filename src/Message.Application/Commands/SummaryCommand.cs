using Message.Application.Models;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class SummaryCommand : IRequest<IEnumerable<ConversationDto>>
{
    /// <summary>
    /// 統計誰的
    /// </summary>
    public long To { get; set; }
}

public class SummaryHandler : IRequestHandler<SummaryCommand,IEnumerable<ConversationDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IItemRepository _itemRepository;

    public SummaryHandler(
        INoteRepository noteRepository, 
        IItemRepository itemRepository)
    {
        _noteRepository = noteRepository;
        _itemRepository = itemRepository;
    }


    public Task<IEnumerable<ConversationDto>> HandleAsync(SummaryCommand request)
    {
        var notes = _noteRepository.Summary(request.To);
        
        var group = notes.GroupBy(x => x.ItemId);

        var ids = group.Select(x => x.Key).ToArray();
        
        var items = _itemRepository.Get(ids);

        var conversations = group.Select((messages) =>
        {
            var item = items.First(x => x.Id == messages.Key);
            
            return new ConversationDto()
            {
                UnreadCount = messages.Count(),
                LastMessage = messages.Last().ToDto(),
                Item = item
            };
        });

        return Task.FromResult(conversations);
    }
}