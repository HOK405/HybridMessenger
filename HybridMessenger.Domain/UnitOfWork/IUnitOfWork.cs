namespace HybridMessenger.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository>() where TRepository : class;

        Task<int> SaveChangesAsync();
    }
}
