using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> VerifyUserByEmailAndPasswordAsync(string email, string password);
    }
}
