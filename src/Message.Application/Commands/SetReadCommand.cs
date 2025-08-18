using Message.Application.Models;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class SetReadCommand : IRequest<bool>
{
    /// <summary>
    /// 連接
    /// </summary>
    public required ConnectionModel Connection { get; set; }
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
        var uri = request.Connection.Uri;
        if (string.IsNullOrEmpty(uri))
            return Task.FromResult(false);
        
        _noteRepository.SetRead(
            request.Connection.UserId, 
            request.Connection.Uri!);
        
        return Task.FromResult(true);
    }
}