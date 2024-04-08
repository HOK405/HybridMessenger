using HybridMessenger.Domain.Repositories;
using System.Linq.Expressions;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected readonly ApiDbContext _context;

        public Repository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<IQueryable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            string searchValue = "",
            bool ascending = true)
        {
            var query = _context.Set<T>().AsQueryable();

            // Filter if necessary
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression filterExpression = null;
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                foreach (var property in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string)))
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var filterConstant = Expression.Constant(searchValue);
                    var containsExpression = Expression.Call(propertyAccess, containsMethod, filterConstant);

                    filterExpression = filterExpression == null
                        ? containsExpression
                        : Expression.OrElse(filterExpression, containsExpression);
                }

                if (filterExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                    query = query.Where(lambda);
                }
            }

            // Sorting
            var sortParameter = Expression.Parameter(typeof(T), "x");
            Expression sortProperty = Expression.Property(sortParameter, sortBy);
            var sortLambda = Expression.Lambda<Func<T, object>>(Expression.Convert(sortProperty, typeof(object)), sortParameter);

            query = ascending ? query.OrderBy(sortLambda) : query.OrderByDescending(sortLambda);

            // Pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query;
        }

    }
}