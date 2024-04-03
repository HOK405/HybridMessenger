using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<User> _userManager;

        public UserIdentityService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"User creation failed: {errors}");
            }
        }

        public async Task<User> VerifyUserByEmailAndPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                throw new ArgumentException("Invalid password.");
            }

            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var userIdAsString = id.ToString();
            var user = await _userManager.FindByIdAsync(userIdAsString);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            return user;
        }
    }
}
