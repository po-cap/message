using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;

namespace Message.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;
    
    public NoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Note note)
    {
        _context.Notes.Add(note);
        _context.SaveChanges();
    }

    /// <summary>
    /// 取得 - 未讀訊息
    /// </summary>
    /// <param name="receiverId">收訊者 ID</param>
    /// <param name="conversationId">對話 ID</param>
    /// <returns></returns>
    public IEnumerable<Note> Get(long receiverId, long conversationId)
    {
        // processing - 
        var notes = _context.Notes
            .Where(x => x.ReceiverId == receiverId)
            .Where(x => x.ConversationId == conversationId)
            .Where(x => x.ReadAt == null)
            .ToList();
        
        // processing - 
        var now = DateTimeOffset.Now;
        foreach(var note in notes)
        {
            note.ReadAt = now;
        }
        _context.SaveChanges();
        
        // return - 
        return notes;
    }
}