using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<string> FindEmailByUsernameAsync(string username);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
