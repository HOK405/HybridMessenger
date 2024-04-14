using FluentValidation;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreatePrivateChatCommandValidator : AbstractValidator<CreatePrivateChatCommand>
    {
        private readonly IUserRepository _userRepository;

        public CreatePrivateChatCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(command => command.UserNameToCreateWith)
                .NotEmpty().WithMessage("Username to create chat with cannot be empty.")
                .MustAsync(UserExists).WithMessage("The user with this username does not exist.");
        }

        private async Task<bool> UserExists(string username, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            return user != null;
        }
    }
}
