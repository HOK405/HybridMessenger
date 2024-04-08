using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class UserRepository : Repository<Domain.Entities.User, Guid>, IUserRepository
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
    }
}