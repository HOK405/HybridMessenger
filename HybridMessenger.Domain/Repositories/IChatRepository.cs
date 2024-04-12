using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IChatRepository : IRepository<Chat, Guid>
    {
        Task<Chat> CreateChatAsync(string chatName, bool isGroup);
        Task<IQueryable<Chat>> GetPagedUserChatsAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            string sortBy,
            string searchValue = "",
            bool ascending = true);
    }
}
