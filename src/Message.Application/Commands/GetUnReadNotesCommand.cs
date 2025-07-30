using Message.Application.Models;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class GetUnReadNotesCommand : IRequest<IEnumerable<MessageDto>>
{
    /// <summary>
    /// 寄給誰的訊息
    /// </summary>
    public long To { get; set; }

    /// <summary>
    /// 誰寄的
    /// </summary>
    public long From { get; set; }
}

public class GetUnReadNotesHandler : IRequestHandler<GetUnReadNotesCommand, IEnumerable<MessageDto>>
{
    private readonly INoteRepository _noteRepository;

    public GetUnReadNotesHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }


    public Task<IEnumerable<MessageDto>> HandleAsync(GetUnReadNotesCommand request)
    {
        var notes = _noteRepository.Get(to: request.To, from: request.From);
        return Task.FromResult(from n in notes select n.ToDto());
    }
}