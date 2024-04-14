using FluentValidation;
using HybridMessenger.Domain.Services;

namespace HybridMessenger.Application.User.Commands
{
    public class VerifyByEmailPasswordCommandValidator : AbstractValidator<VerifyByEmailPasswordCommand>
    {
        private readonly IUserIdentityService _userService;

        public VerifyByEmailPasswordCommandValidator(IUserIdentityService userService)
        {
            _userService = userService;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x).CustomAsync(ValidateCredentialsAsync);
        }

        private async Task ValidateCredentialsAsync(VerifyByEmailPasswordCommand command, ValidationContext<VerifyByEmailPasswordCommand> context, CancellationToken cancellationToken)
        {
            var user = await _userService.VerifyUserByEmailAndPasswordAsync(command.Email, command.Password);
            if (user == null)
            {
                context.AddFailure("Email or Password", "Invalid email or password.");
            }
        }
    }
}
