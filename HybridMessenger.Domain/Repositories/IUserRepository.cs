using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<User> GetUserByUsernameAsync(string username);
    }
}
