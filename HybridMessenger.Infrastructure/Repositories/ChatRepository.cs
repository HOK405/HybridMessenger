using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ChatRepository : Repository<Chat, Guid>, IChatRepository
    {
        public ChatRepository(ApiDbContext context) : base(context)
        {
        }

        public async Task<Chat> CreateChatAsync(string chatName, bool isGroup)
        {
            Guid newChatId = Guid.NewGuid();

            var chat = new Chat
            {
                ChatID = newChatId,
                ChatName = isGroup ? chatName : null,
                IsGroup = isGroup,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Chats.AddAsync(chat);

            return chat;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Chats.AnyAsync(c => c.ChatID == id);
        }

        public async Task<IQueryable<Chat>> GetPagedUserChatsAsync(
            Guid userId, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            string searchValue = "", 
            bool ascending = true)
        {
            var userChatsQuery = _context.ChatMembers
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.Chat);

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                userChatsQuery = ApplySearch(userChatsQuery, searchValue);
            }

            // Apply dynamic sorting
            userChatsQuery = ApplySorting(userChatsQuery, sortBy, ascending);

            // Apply pagination
            userChatsQuery = ApplyPagination(userChatsQuery, pageNumber, pageSize);

            return userChatsQuery;
        }
    }
}
