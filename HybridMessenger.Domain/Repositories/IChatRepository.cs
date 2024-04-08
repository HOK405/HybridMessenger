using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IChatRepository : IRepository<Chat, Guid>
    {
    }
}
