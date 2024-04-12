using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IChatMemberRepository : IRepository<ChatMember, (Guid, Guid)>
    {
        Task<ChatMember> AddUserToChatAsync(User user, Chat chat);
    }
}