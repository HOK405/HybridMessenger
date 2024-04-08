using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message, int>, IMessageRepository
    {
        public MessageRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
