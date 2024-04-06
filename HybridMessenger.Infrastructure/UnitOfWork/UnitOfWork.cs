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
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApiDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(IGenericRepository<>).MakeGenericType(typeof(T));
                var repositoryInstance = Activator.CreateInstance(typeof(UserRepository), _context);
                _repositories[type] = repositoryInstance;
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        private object CreateSpecificRepository<T>() where T : class
        {
            // Attempt to find a custom repository type.
            var customRepoType = typeof(IGenericRepository<>).Assembly.GetTypes()
                .FirstOrDefault(t => typeof(IGenericRepository<T>).IsAssignableFrom(t) && !t.IsInterface);

            if (customRepoType != null)
            {
                return Activator.CreateInstance(customRepoType, _context);
            }

            return null;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
