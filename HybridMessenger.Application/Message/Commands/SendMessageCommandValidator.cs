using FluentValidation;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;
        public SendMessageCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();

            RuleFor(command => command.ChatId)
               .NotEmpty().WithMessage("Chat ID cannot be empty")
               .Must(BeAValidGuid).WithMessage("Chat ID must be a valid GUID")
               .MustAsync(async (chatId, cancellation) => await ChatExists(chatId)).WithMessage("Invalid chat id.");

            RuleFor(command => command.MessageText)
                .NotEmpty().WithMessage("Message cannot be empty");

        }

        private bool BeAValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }

        private async Task<bool> ChatExists(string chatId)
        {
            return await _chatRepository.ExistsAsync(Guid.Parse(chatId));
        }      
    }
}
