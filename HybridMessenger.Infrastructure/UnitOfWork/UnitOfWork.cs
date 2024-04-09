using HybridMessenger.Domain.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace HybridMessenger.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _disposed = false; 

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

        public async Task<int> SaveChangesAsync()
        {
            var dbContext = _serviceProvider.GetService<ApiDbContext>();
            if (dbContext == null)
            {
                throw new InvalidOperationException("DbContext not registered in the service provider");
            }

            return await dbContext.SaveChangesAsync();
        }

/*        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var dbContext = _serviceProvider.GetService<ApiDbContext>();
                    dbContext?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }*/
    }
}
