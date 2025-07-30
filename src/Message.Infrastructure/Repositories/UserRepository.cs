using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Services;

namespace Message.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthClient _authClient;

    public UserRepository(AuthClient authClient)
    {
        _authClient = authClient;
    }

    public Task<User> GetAsync(string token)
    {
        return _authClient.GetAsync(token);
    }
}