using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetAllUnreadNotesQuery : IRequest<MessageModel>
{
    /// <summary>
    /// 用戶連線資訊
    /// </summary>
    public required ConnectionModel Connection { get; set; }
}

public class GetAllUnreadNotesHandler : IRequestHandler<GetAllUnreadNotesQuery, MessageModel>
{
    private readonly AppDbContext _context;

    public GetAllUnreadNotesHandler(AppDbContext context)
    {
        _context = context;
    }

    public Task<MessageModel> HandleAsync(GetAllUnreadNotesQuery request)
    {
        //  
        var unreadMessages = _context.Notes
            .Where(x => x.ReceiverId == request.Connection.UserId && x.ReadAt == null)
            .ToList();
        
        //
        var content = JsonSerializer.Serialize(
            from m in  unreadMessages select m.ToModel(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        //
        return Task.FromResult(new MessageModel
        {
            Id = 0,
            Uri = "",
            ReceiverId = request.Connection.UserId,
            Content = content,
            Type = NoteType.unread_messages,
            Status = 0
        });
    }
}