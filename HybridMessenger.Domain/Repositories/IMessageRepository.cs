using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Message> GetByIdAsync(int id);
    }
}
