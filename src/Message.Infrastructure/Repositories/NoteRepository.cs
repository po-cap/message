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
}