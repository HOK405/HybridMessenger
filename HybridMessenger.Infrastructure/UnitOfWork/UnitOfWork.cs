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
        private readonly UserManager<User> _userManager;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(ApiDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                if (type == typeof(Domain.Entities.User))
                {
                    var repositoryInstance = new UserRepository(_context, _userManager);
                    _repositories[type] = repositoryInstance;
                }
                else
                {
                    _repositories[type] = new GenericRepository<T>(_context);
                }
            }

            return (IRepository<T>)_repositories[type];
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
