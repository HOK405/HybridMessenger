using FluentValidation;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
    {
        private readonly IChatRepository _chatRepository;

        public DeleteChatCommandValidator(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;

            RuleFor(command => command.ChatId)
                .Must(BeAValidGuid).WithMessage("The provided UserId must be a valid GUID.")
                .MustAsync(ChatExists).WithMessage("The chat with this ID does not exist.");
        }

        private bool BeAValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }

        private async Task<bool> ChatExists(string chatId, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(chatId));
            return chat != null;
        }
    }
}
