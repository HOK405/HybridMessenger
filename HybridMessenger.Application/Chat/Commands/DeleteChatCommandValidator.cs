using FluentValidation;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
    {
        private readonly IChatRepository _chatRepository;

        public DeleteChatCommandValidator(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;

            RuleFor(command => command.ChatId)
                .MustAsync(ChatExists).WithMessage("The chat with this ID does not exist.");
        }


        private async Task<bool> ChatExists(int chatId, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            return chat != null;
        }
    }
}
