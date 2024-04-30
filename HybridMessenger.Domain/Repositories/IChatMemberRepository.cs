using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Repositories
{
    public interface IChatMemberRepository : IRepository<ChatMember>
    {
        Task<ChatMember> AddUserToChatAsync(User user, Chat chat);

        Task RemoveUserFromChatAsync(User user, Chat chat);

        Task<bool> IsUserMemberOfChatAsync(int userId, int chatId);
    }
}