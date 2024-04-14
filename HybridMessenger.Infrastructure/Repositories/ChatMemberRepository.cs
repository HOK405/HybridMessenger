using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatMemberRepository : Repository<ChatMember, (Guid, Guid)>, IChatMemberRepository
    {
        public ChatMemberRepository(ApiDbContext context) : base(context) 
        { 
        }

        public async Task<bool> IsUserMemberOfGroupAsync(Guid userId, Guid chatId)
        {
            return await _context.ChatMembers.AnyAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
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

            return newMember;
        }
    }
}
