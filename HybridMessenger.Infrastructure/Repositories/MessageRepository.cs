using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
