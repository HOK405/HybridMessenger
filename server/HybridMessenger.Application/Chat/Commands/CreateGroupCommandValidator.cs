using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(cmd => cmd.ChatName)
                .NotEmpty().WithMessage("Chat name is required.")
                .Length(1, 255).WithMessage("Chat name must be between 1 and 255 characters.");
        }
    }
}
