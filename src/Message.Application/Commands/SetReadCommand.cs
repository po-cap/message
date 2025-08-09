using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class SetReadCommand : IRequest<bool>
{
    /// <summary>
    /// 訊息的 ID
    /// </summary>
    public required IEnumerable<long> Ids { get; set; }
}


public class SetReadHandler : IRequestHandler<SetReadCommand, bool>
{
    private readonly INoteRepository _noteRepository;

    public SetReadHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }


    public Task<bool> HandleAsync(SetReadCommand request)
    {
        _noteRepository.SetRead(request.Ids);
        return Task.FromResult(true);
    }
}