using Message.Application.Models;
using Message.Application.Services;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class ExitTheChatCommand : IRequest<bool>
{
    /// <summary>
    /// 使用者連結
    /// </summary>
    public required ConnectionModel Connection { get; set; }
}


public class ExitTheChatHandler : IRequestHandler<ExitTheChatCommand, bool>
{
    private readonly IConnection _connection;

    public ExitTheChatHandler(IConnection connection)
    {
        _connection = connection;
    }
    
    public Task<bool> HandleAsync(ExitTheChatCommand request)
    {
        request.Connection.Uri = null;
        request.Connection.PartnerId = null;
        return Task.FromResult(false);
    }
}
