using FluentValidation;
using HybridMessenger.Domain.Services;

namespace HybridMessenger.Application.User.Commands
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        private readonly IUserIdentityService _userIdentityService;
        public DeleteUserCommandValidator(IUserIdentityService userService)
        {
            _userIdentityService = userService;

            RuleFor(x => x.UserIdToDelete)
                .NotEmpty().WithMessage("User ID can't be empty.")
                .MustAsync(UserExists).WithMessage("User doesn't exist");
        }

        private async Task<bool> UserExists(int id, CancellationToken cancellation)
        {
            var user = await _userIdentityService.GetUserByIdAsync(id);

            if (user is null)
            {
                return false;
            }

            return true;
        }
    }
}
