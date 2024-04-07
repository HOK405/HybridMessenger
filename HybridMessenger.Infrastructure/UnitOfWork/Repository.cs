using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HybridMessenger.Infrastructure.UnitOfWork
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

        public async Task<IQueryable<T>> GetPagedAsync(int pageNumber, int pageSize, string sortBy, bool ascending = true)
        {
            var query = _context.Set<T>().AsQueryable();

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            // Apply sorting
            query = ascending ? query.OrderBy(lambda) : query.OrderByDescending(lambda);

            // Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query; 
        }
    }
}