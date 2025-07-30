using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Services;

namespace Message.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;
    private readonly SnowflakeId _snowflake;
    
    public NoteRepository(
        SnowflakeId snowflake, 
        AppDbContext context)
    {
        _snowflake = snowflake;
        _context = context;
    }

    public void Add(Note note)
    {
        note.Id = _snowflake.Get();
        
        _context.Notes.Add(note);
        _context.SaveChanges();
    }

    public IEnumerable<Note> Get(long to, long from)
    {
        // processing - 
        var notes = _context.Notes
            .Where(x => x.ReceiverId == to)
            .Where(x => x.ReadAt == null)
            .Where(x => x.SenderId == from)
            .ToList();
        
        // processing - 
        var now = DateTimeOffset.Now;
        foreach(var note in notes)
        {
            note.ReadAt = now;
            note.GetAt = now;
        }
        _context.SaveChanges();
        
        // return - 
        return notes;
    }

    IEnumerable<Note> INoteRepository.Summary(long to)
    {
        // processing -
        var notes = _context.Notes
            .Where(x => x.ReceiverId == to)
            .Where(x => x.GetAt == null);

        // processing - 
        var now = DateTimeOffset.Now;
        foreach(var note in notes)
        {
            note.GetAt = now;
        }
        _context.SaveChanges();
        
        // return - 
        return notes;
    }
}