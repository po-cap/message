using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Po.Api.Response;

namespace Message.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _dbContext;

    public ItemRepository( AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Item Get(long id)
    {
        var item = _dbContext.Items.Find(id);
        if(item == null)
            throw Failure.BadRequest(title: "商品鏈結不存在");
        
        _dbContext.Entry(item).Reference(x => x.User).Load();
        
        return item;
    }
}