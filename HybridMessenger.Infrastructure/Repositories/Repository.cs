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

        public async void AddAsync(T entity)
        {
           await  _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<IQueryable<T>> GetPagedAsync(int pageNumber,
                                                       int pageSize,
                                                       string sortBy,
                                                       Dictionary<string, object> filters = null,
                                                       string searchValue = "",
                                                       bool ascending = true)
        {
            var query = _context.Set<T>().AsQueryable();

            // Dynamic filtering based on specific fields
            if (filters != null && filters.Any())
            {
                query = ApplyFilters(query, filters);
            }

            // Searching
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = ApplySearch(query, searchValue);
            }

            // Sorting
            query = ApplySorting(query, sortBy, ascending);
            
            // Pagination
            query = ApplyPagination(query, pageNumber, pageSize);

            return query;
        }

        protected IQueryable<T> ApplySearch(IQueryable<T> query, string searchValue)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression searchExpression = null;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var toStringMethod = typeof(object).GetMethod("ToString");

            foreach (var property in typeof(T).GetProperties())
            {
                // Skip non-string and non-Guid properties
                if (property.PropertyType != typeof(string) && property.PropertyType != typeof(Guid))
                {
                    continue;
                }

                Expression propertyAccess = Expression.Property(parameter, property);
                // If the property is a Guid, convert it to string
                if (property.PropertyType == typeof(Guid))
                {
                    propertyAccess = Expression.Call(propertyAccess, toStringMethod);
                }

                var searchConstant = Expression.Constant(searchValue);
                var containsExpression = Expression.Call(propertyAccess, containsMethod, searchConstant);

                searchExpression = searchExpression == null
                    ? containsExpression
                    : Expression.OrElse(searchExpression, containsExpression);
            }

            if (searchExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        protected IQueryable<T> ApplyPagination(IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        protected IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy, bool ascending)
        {
            var sortParameter = Expression.Parameter(typeof(T), "x");
            Expression sortProperty = Expression.Property(sortParameter, sortBy);
            var sortLambda = Expression.Lambda<Func<T, object>>(Expression.Convert(sortProperty, typeof(object)), sortParameter);

            return ascending ? query.OrderBy(sortLambda) : query.OrderByDescending(sortLambda);
        }

        protected IQueryable<T> ApplyFilters(IQueryable<T> query, Dictionary<string, object> filters)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression filterExpression = null;

            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key);
                if (property != null)
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var filterConstant = Expression.Constant(filter.Value, property.PropertyType);
                    var conditionExpression = Expression.Equal(propertyAccess, filterConstant);

                    filterExpression = filterExpression == null
                        ? conditionExpression
                        : Expression.AndAlso(filterExpression, conditionExpression);
                }
            }

            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }
    }
}