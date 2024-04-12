using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(ApiDbContext context) : base(context) { }

        public async Task<string> FindEmailByUsernameAsync(string username)
        {
            var userEmail = await _context.Users
                .Where(u => u.UserName == username) 
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            return userEmail; 
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}