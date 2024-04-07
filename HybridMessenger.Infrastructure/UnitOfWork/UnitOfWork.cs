using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using HybridMessenger.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HybridMessenger.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repositoryInstance = _serviceProvider.GetService<TRepository>();
            if (repositoryInstance == null)
            {
                throw new InvalidOperationException($"Repository not registered for type {typeof(TRepository).Name}");
            }
            return repositoryInstance;
        }

        public IRepository<T, TKey> GetRepositoryForEntity<T, TKey>() where T : class
        {
            var entityType = typeof(T);
            var repositoryType = typeof(IRepository<,>).MakeGenericType(entityType, typeof(TKey));
            var repositoryInstance = _serviceProvider.GetService(repositoryType);
            if (repositoryInstance == null)
            {
                throw new InvalidOperationException($"Repository not registered for entity type {entityType.Name}");
            }
            return (IRepository<T, TKey>)repositoryInstance;
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
