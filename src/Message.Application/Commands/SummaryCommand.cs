using Message.Application.Models;
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

    public SummaryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }


    public Task<IEnumerable<ConversationDto>> HandleAsync(SummaryCommand request)
    {
        var notes = _noteRepository.Summary(request.To);

        return Task.FromResult(notes.GroupBy(
            x => x.SenderId,
            (conversation, messages) => new ConversationDto()
            {
                SenderId = conversation,
                UnReadCount = messages.Count(),
                LastMessage = messages.Last().ToDto(),
                SenderName = messages.First().SenderName,
                SenderAvatar = messages.First().SenderAvatar,
            })); 
    }
}