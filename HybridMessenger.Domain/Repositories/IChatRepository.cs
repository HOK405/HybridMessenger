using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task<Chat> CreateChatAsync(string chatName, bool isGroup);

        Task<bool> ExistsAsync(int id);

        Task<IQueryable<Chat>> GetPagedUserChatsAsync(
            int userId,
            int pageNumber,
            int pageSize,
            string sortBy,
            string searchValue = "",
            bool ascending = true);
    }
}
