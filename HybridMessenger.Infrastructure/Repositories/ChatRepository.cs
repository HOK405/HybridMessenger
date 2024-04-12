using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using System.Linq.Expressions;

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
            /*await _context.SaveChangesAsync();*/

            return chat;
        }

        public async Task<IQueryable<Chat>> GetPagedUserChatsAsync(Guid userId, int pageNumber, int pageSize, string sortBy, string searchValue = "", bool ascending = true)
        {
            // Start with querying the ChatMembers to filter by the specified UserId
            var userChatsQuery = _context.ChatMembers
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.Chat);

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                userChatsQuery = ApplySearch(userChatsQuery, searchValue);
            }

            // Apply dynamic sorting
            var sortParameter = Expression.Parameter(typeof(Chat), "x");
            Expression sortProperty = Expression.Property(sortParameter, sortBy);
            var sortLambda = Expression.Lambda<Func<Chat, object>>(Expression.Convert(sortProperty, typeof(object)), sortParameter);

            userChatsQuery = ascending ? userChatsQuery.OrderBy(sortLambda) : userChatsQuery.OrderByDescending(sortLambda);

            // Apply pagination
            userChatsQuery = userChatsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return userChatsQuery;
        }

        private IQueryable<Chat> ApplySearch(IQueryable<Chat> query, string searchValue)
        {
            var parameter = Expression.Parameter(typeof(Chat), "x");
            Expression searchExpression = null;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            foreach (var property in typeof(Chat).GetProperties())
            {
                if (property.PropertyType != typeof(string))
                {
                    continue;
                }

                var propertyAccess = Expression.Property(parameter, property);
                var searchConstant = Expression.Constant(searchValue);
                var containsExpression = Expression.Call(propertyAccess, containsMethod, searchConstant);

                searchExpression = searchExpression == null ? containsExpression : Expression.OrElse(searchExpression, containsExpression);
            }

            if (searchExpression != null)
            {
                var lambda = Expression.Lambda<Func<Chat, bool>>(searchExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

    }
}
