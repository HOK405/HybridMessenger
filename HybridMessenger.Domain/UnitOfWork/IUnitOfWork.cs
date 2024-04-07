using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Domain.UnitOfWork
{
    public interface IUnitOfWork
    {
        TRepository GetRepository<TRepository>() where TRepository : class;

        IRepository<T, TKey> GetRepositoryForEntity<T, TKey>() where T : class;

        Task<int> SaveChangesAsync();
    }
}
