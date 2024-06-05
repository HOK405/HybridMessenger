using FluentValidation;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Chat.Commands
{
    public class RemoveGroupMemberCommandValidator : AbstractValidator<RemoveGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IChatMemberRepository _chatMemberRepository;

        public RemoveGroupMemberCommandValidator(IUnitOfWork unitOfWork, IUserIdentityService userIdentityService)
        {
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();

            RuleFor(cmd => cmd.UserIdToRemove)
                .NotEmpty().WithMessage("User ID cannot be empty.")
                .MustAsync(UserExists).WithMessage("The specified user does not exist.")
                .MustAsync(UserInChat).WithMessage("The user is not in this chat.");

            RuleFor(cmd => cmd.ChatId)
                .NotEmpty().WithMessage("Chat ID cannot be empty.");
        }

        private async Task<bool> UserExists(int userId, CancellationToken cancellation)
        {
            return await _userIdentityService.GetUserByIdAsync(userId) != null;
        }

        private async Task<bool> UserInChat(RemoveGroupMemberCommand command, int userIdToRemove, CancellationToken cancellation)
        {
            return await _chatMemberRepository.IsUserMemberOfChatAsync(userIdToRemove, command.ChatId);
        }
    }
}
