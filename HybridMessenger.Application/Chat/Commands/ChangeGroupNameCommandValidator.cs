using FluentValidation;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeGroupNameCommandValidator : AbstractValidator<ChangeGroupNameCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;

        public ChangeGroupNameCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();

            RuleFor(command => command.ChatId)
                .NotEmpty().WithMessage("Chat ID cannot be empty")
                .Must(BeAValidGuid).WithMessage("Chat ID must be a valid GUID")
                .MustAsync(async (chatId, cancellation) => await ChatExists(chatId)).WithMessage("Invalid chat id.")
                .MustAsync(async (chatId, cancellation) => await IsGroupChat(chatId))
                .WithMessage("Only group chats can have their names changed.");

            RuleFor(command => command.NewChatName)
                .NotEmpty().WithMessage("New chat name cannot be empty")
                .Length(1, 255).WithMessage("New chat name must be between 1 and 255 characters");
        }

        private bool BeAValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }

        private async Task<bool> ChatExists(string chatId)
        {
            return await _chatRepository.ExistsAsync(Guid.Parse(chatId));
        }

        private async Task<bool> IsGroupChat(string chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(chatId));
            return chat.IsGroup;
        }
    }
}
