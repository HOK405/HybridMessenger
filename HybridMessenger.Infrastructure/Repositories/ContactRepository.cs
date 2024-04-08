using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Infrastructure.Repositories
{
    public class ContactRepository : Repository<Contact, (Guid, Guid)>, IContactRepository
    {
        public ContactRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
