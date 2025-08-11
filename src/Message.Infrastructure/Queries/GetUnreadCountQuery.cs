using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Shared.Mediator.Interface;

namespace Message.Infrastructure.Queries;

public class GetUnreadCountQuery : IRequest<MessageModel>
{
    /// <summary>
    /// 使用者連線資訊
    /// </summary>
    public required ConnectionModel Connection { get; set; }
}


public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, MessageModel>
{
    private readonly AppDbContext _context;

    public GetUnreadCountHandler(AppDbContext context)
    {
        _context = context;
    }

    public Task<MessageModel> HandleAsync(GetUnreadCountQuery request)
    {
        // 向資料庫要所有使用者的未讀訊息 
        var unreadMessages = _context.Notes
            .Where(x => x.ReceiverId == request.Connection.UserId && x.ReadAt == null)
            .ToList();
        
        return Task.FromResult(new MessageModel
        {
            Id = 0,
            Uri = "",
            ReceiverId = 0,
            Content = $"{unreadMessages.Count}",
            Type = NoteType.unread_count,
            Status = 0
        });
    }
}