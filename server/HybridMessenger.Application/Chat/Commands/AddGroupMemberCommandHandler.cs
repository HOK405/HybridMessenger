using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class AddGroupMemberCommandHandler : IRequestHandler<AddGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatMemberRepository _chatMemberRepository;

        public AddGroupMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _userRepository = _unitOfWork.GetRepository<IUserRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();
        }

        public async Task Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
        {
            if (!await UserIsInChat(request.UserId, request.ChatId))
            {
                throw new InvalidOperationException("User is not a member of the specified chat.");
            }

            var userToAdd = await _userRepository.GetUserByUsernameAsync(request.UserNameToAdd);

            var chatToModify = await _chatRepository.GetByIdAsync(request.ChatId);

            await _chatMemberRepository.AddUserToChatAsync(userToAdd, chatToModify);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> UserIsInChat(int userId, int chatId)
        {
            return await _chatMemberRepository.IsUserMemberOfChatAsync(userId, chatId);
        }
    }
}
