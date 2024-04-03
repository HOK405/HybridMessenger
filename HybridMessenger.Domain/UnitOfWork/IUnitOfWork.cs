using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> CommitAsync();
    }
}
