using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IQueryable<Message>> GetPagedMessagesAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            Dictionary<string, object> filters = null,
            string searchValue = "",
            bool ascending = true);
    }
}
