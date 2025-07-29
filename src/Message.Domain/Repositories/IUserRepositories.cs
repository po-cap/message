using Message.Domain.Entities;

namespace Message.Domain.Repositories;

public interface IUserRepositories
{
    Task<User> GetUserByIdAsync(string id);
}