using Microsoft.EntityFrameworkCore.Storage;

namespace HybridMessenger.Domain.UnitOfWork
{
    public interface IUnitOfWork 
    {
        TRepository GetRepository<TRepository>() where TRepository : class;
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
