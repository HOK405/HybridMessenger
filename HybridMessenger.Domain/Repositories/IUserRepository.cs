using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
