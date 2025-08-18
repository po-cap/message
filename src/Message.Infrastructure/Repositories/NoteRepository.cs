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
    

    public void SetRead(long userId, string uri)
    {
        var component = uri.Split("/");
        var buyerId   = long.Parse(component[0]);
        var itemId    = long.Parse(component[1]);

        var notes = _context.Notes.Where(x => x.BuyerId == buyerId && x.ItemId == itemId && x.ReadAt == null);
        
        var now = DateTimeOffset.Now;
        foreach (var note in notes)
        {
            note.ReadAt = now;
        }

        _context.SaveChanges();
    }
}