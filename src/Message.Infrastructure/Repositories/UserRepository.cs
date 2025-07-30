using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Services;

namespace Message.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<User> Get(long[] ids)
    {
        return _dbContext.Users.Where(x => ids.Contains(x.Id)).ToList();
    }
}