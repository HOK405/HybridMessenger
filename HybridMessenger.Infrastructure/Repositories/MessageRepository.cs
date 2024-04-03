using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApiDbContext context) : base(context)
        {
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(u => u.MessageID == id);
        }
    }
}
