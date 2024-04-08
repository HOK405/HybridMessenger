using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IContactRepository : IRepository<Contact, (Guid, Guid)>
    {
    }
}
