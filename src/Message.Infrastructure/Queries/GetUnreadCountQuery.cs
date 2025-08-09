using Amazon.Runtime.Internal;
using Message.Application.Models;
using Message.Infrastructure.Persistence;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetUnreadCountQuery : IRequest<int>
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public long UserId { get; set; }
}


public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly AppDbContext _context;

    public GetUnreadCountHandler(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> HandleAsync(GetUnreadCountQuery request)
    {
        // processing - 
        var unreadMessages = _context.Notes
            .Where(x => x.ReceiverId == request.UserId && x.ReadAt == null)
            .ToList();

        return Task.FromResult(unreadMessages.Count);
    }
}