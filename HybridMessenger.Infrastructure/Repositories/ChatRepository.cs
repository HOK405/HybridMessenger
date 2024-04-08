using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatRepository : Repository<Chat, Guid>, IChatRepository
    {
        public ChatRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
