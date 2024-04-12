using FluentValidation;
using HybridMessenger.Domain.Repositories;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeChatNameCommandValidator : AbstractValidator<ChangeChatNameCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;

        public ChangeChatNameCommandValidator(IChatRepository chatRepository, IChatMemberRepository chatMemberRepository)
        {
            _chatRepository = chatRepository;
            _chatMemberRepository = chatMemberRepository;

            RuleFor(command => command.UserId)
                .NotEmpty().WithMessage("User ID cannot be empty")
            .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID")
                .MustAsync(async (command, userId, cancellation) => await UserIsInChat(userId, command.ChatId)).WithMessage("User is not a member of the specified chat.");

            RuleFor(command => command.ChatId)
                .NotEmpty().WithMessage("Chat ID cannot be empty")
                .Must(BeAValidGuid).WithMessage("Chat ID must be a valid GUID")
                .MustAsync(async (chatId, cancellation) => await ChatExists(chatId)).WithMessage("Invalid chat id.");

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
            return await _chatRepository.ExistsAsync(Guid.Parse(chatId), e => e.ChatID);
        }

        private async Task<bool> UserIsInChat(string userId, string chatId)
        {
            return await _chatMemberRepository.IsUserMemberOfChatAsync(Guid.Parse(userId), Guid.Parse(chatId));
        }
    }
}
