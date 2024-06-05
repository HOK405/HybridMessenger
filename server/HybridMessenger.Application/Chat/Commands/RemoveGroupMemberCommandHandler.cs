using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class RemoveGroupMemberCommandHandler : IRequestHandler<RemoveGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;
        private readonly IUserIdentityService _userIdentityService;

        public RemoveGroupMemberCommandHandler(IUnitOfWork unitOfWork, IUserIdentityService userIdentityService)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();
            _userIdentityService = userIdentityService;
        }

        public async Task Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
        {
            if (!await UserIsInChat(request.UserId, request.ChatId))
            {
                throw new InvalidOperationException("User is not a member of the specified chat.");
            }

            var userToRemove = await _userIdentityService.GetUserByIdAsync(request.UserIdToRemove);
            if (userToRemove == null)
            {
                throw new InvalidOperationException("The specified user does not exist.");
            }

            var chat = await _chatRepository.GetByIdAsync(request.ChatId);
            await _chatMemberRepository.RemoveUserFromChatAsync(userToRemove, chat);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> UserIsInChat(int userId, int chatId)
        {
            return await _chatMemberRepository.IsUserMemberOfChatAsync(userId, chatId);
        }
    }
}
