using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Message.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _dbContext;

    public ItemRepository( AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Item> Get(long[] ids)
    {
        return _dbContext.Items
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.User)
            .ToList();
    }
}