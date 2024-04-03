using HybridMessenger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Infrastructure.UnitOfWork
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApiDbContext _context;

        public GenericRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
