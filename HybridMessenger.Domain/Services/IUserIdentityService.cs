using HybridMessenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Domain.Services
{
    public interface IUserIdentityService
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User> VerifyUserByEmailAndPasswordAsync(string email, string password);
        Task<User> GetUserByIdAsync(Guid id);
        Task<IdentityResult> AddRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
    }
}
