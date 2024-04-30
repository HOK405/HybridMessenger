using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatMemberRepository : Repository<ChatMember>, IChatMemberRepository
    {
        public ChatMemberRepository(ApiDbContext context) : base(context) 
        { 
        }

        public async Task<bool> IsUserMemberOfChatAsync(int userId, int chatId)
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

        public async Task RemoveUserFromChatAsync(User user, Chat chat)
        {
            var member = await _context.ChatMembers.FirstOrDefaultAsync(cm => cm.ChatId == chat.ChatId && cm.UserId == user.Id);
            if (member != null)
            {
                _context.ChatMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }
    }
}
