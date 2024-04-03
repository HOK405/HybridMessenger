using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using HybridMessenger.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiDbContext _context;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(ApiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var entityType = typeof(T);
            if (!_repositories.ContainsKey(entityType))
            {
                _repositories[entityType] = new GenericRepository<T>(_context);
            }

            return (IRepository<T>)_repositories[entityType];
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
