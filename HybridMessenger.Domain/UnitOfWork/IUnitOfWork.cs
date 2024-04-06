using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> CommitAsync();
    }
}
