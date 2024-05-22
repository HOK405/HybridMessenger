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

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"User creation failed: {errors}");
            }

            return result;
        }

        public async Task<User> VerifyUserByEmailAndPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return null;
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<IdentityResult> AddRoleAsync(User user, string role)
        {
            try
            {
                return await _userManager.AddToRoleAsync(user, role);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> UpdateUser(User updatedUser)
        {
            var result = await _userManager.UpdateAsync(updatedUser);

            return result.Succeeded;
        }

        public async Task<IdentityResult> DeleteUserByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException($"No user found with ID {id}.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User deletion failed: {errors}");
            }

            return result;
        }
    }
}
