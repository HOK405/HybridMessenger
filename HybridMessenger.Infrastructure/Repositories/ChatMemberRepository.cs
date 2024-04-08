using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatMemberRepository : Repository<ChatMember, (Guid, Guid)>, IChatMemberRepository
    {
        public ChatMemberRepository(ApiDbContext context) : base(context) 
        { 
        }
    }
}
