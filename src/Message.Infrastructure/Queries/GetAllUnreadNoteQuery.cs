using Message.Application.Models;
using Message.Infrastructure.Persistence;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetAllUnreadNoteQuery : IRequest<IEnumerable<MessageModel>>
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public required long UserId { get; set; }
}

public class GetAllUnreadNoteHandler : IRequestHandler<GetAllUnreadNoteQuery, IEnumerable<MessageModel>>
{
    private readonly AppDbContext _context;

    public GetAllUnreadNoteHandler(AppDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<MessageModel>> HandleAsync(GetAllUnreadNoteQuery request)
    {
        // processing - 
        var unreadMessages = _context.Notes
            .Where(x => x.ReceiverId == request.UserId && x.ReadAt == null)
            .ToList();

        return Task.FromResult(unreadMessages.Select(x => x.ToModel()));
    }
}