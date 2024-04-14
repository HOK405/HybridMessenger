using FluentValidation;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Chat.Commands
{
    public class AddGroupMemberCommandValidator : AbstractValidator<AddGroupMemberCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatMemberRepository _chatMemberRepository;

        public AddGroupMemberCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _userRepository = _unitOfWork.GetRepository<IUserRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();

            RuleFor(cmd => cmd.UserNameToAdd)
                .NotEmpty().WithMessage("Username cannot be empty.")
                .MustAsync(ValidUsername)
                .WithMessage("The provided username is not valid.")
                .MustAsync(UserNotInChat)
                .WithMessage("The provided user is already in this chat.");

            RuleFor(cmd => cmd.ChatId)
                .NotEmpty().WithMessage("Chat ID cannot be empty.")
                .MustAsync(ValidStringId)
                .WithMessage("The provided ChatId is not a valid GUID.")
                .MustAsync(IsGroupChat)
                .WithMessage("New users can't be added to private chats.");           
        }

        private async Task<bool> ValidUsername(string username, CancellationToken cancellation)
        {
            return await _userRepository.GetUserByUsernameAsync(username) != null;
        }

        private async Task<bool> ValidStringId(string chatId, CancellationToken cancellation)
        {
            return Guid.TryParse(chatId, out _);
        }

        private async Task<bool> IsGroupChat(string chatId, CancellationToken cancellation)
        {
            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(chatId));
            return chat != null && chat.IsGroup;
        }

        private async Task<bool> UserNotInChat(AddGroupMemberCommand command, string userNameToAdd, CancellationToken cancellation)
        {
            var userToAdd = await _userRepository.GetUserByUsernameAsync(userNameToAdd);
            if (userToAdd == null) return false;
            return !await _chatMemberRepository.IsUserMemberOfGroupAsync(userToAdd.Id, Guid.Parse(command.ChatId));
        }
    }
}
