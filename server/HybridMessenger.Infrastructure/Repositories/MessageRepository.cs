using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApiDbContext context) : base(context)
        {
        }

        public async Task<bool> IsUserMessageAsync(int messageId, int userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return false;
            }

            return message.UserId == userId;
        }

        public async Task<IQueryable<Message>> GetPagedMessagesAsync(
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            Dictionary<string, object> filters = null, 
            string searchValue = "", 
            bool ascending = true)
        {
            var query = _context.Set<Message>().AsQueryable();

            query = query.Include(m => m.User);

            if (filters != null && filters.Any())
            {
                query = ApplyFilters(query, filters);
            }

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = ApplySearch(query, searchValue);
            }

            query = ApplySorting(query, sortBy, ascending);
            query = ApplyPagination(query, pageNumber, pageSize);

            return query;
        }
    }
}
