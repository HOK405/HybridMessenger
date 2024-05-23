using FluentValidation;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HybridMessenger.Application.User.Commands
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.NewUsername)
                .NotEmpty()
                .WithMessage("The provided username is empty");

            RuleFor(x => x.NewPhoneNumber)
                .NotEmpty()
                .WithMessage("The provided phone number is empty");
        }
    }

}