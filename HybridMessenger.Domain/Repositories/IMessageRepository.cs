using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IMessageRepository : IRepository<Message, int>
    {
    }
}
