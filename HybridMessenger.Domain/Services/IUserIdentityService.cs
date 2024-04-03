namespace HybridMessenger.Domain.Services
{
    public interface IUserIdentityService
    {
        Task CreateUserAsync(Entities.User user, string password);
        Task<Entities.User> VerifyUserByEmailAndPasswordAsync(string email, string password);
        Task<Entities.User> GetUserByIdAsync(Guid id);
    }
}
