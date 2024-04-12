using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatMemberRepository : Repository<ChatMember, (Guid, Guid)>, IChatMemberRepository
    {
        public ChatMemberRepository(ApiDbContext context) : base(context) 
        { 
        }

        public async Task<ChatMember> AddUserToChatAsync(User user, Chat chat)
        {
            ChatMember newMember = new ChatMember()
            {
                Chat = chat,
                User = user,
                JoinedAt = DateTime.UtcNow
            };

            await _context.ChatMembers.AddAsync(newMember);       
            await _context.SaveChangesAsync();

            return newMember;
        }
    }
}
