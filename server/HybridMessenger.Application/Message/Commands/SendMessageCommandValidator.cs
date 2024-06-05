using FluentValidation;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        public SendMessageCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();

            RuleFor(command => command.ChatId)
               .NotEmpty().WithMessage("Chat ID cannot be empty")
               .MustAsync(async (chatId, cancellation) => await ChatExists(chatId)).WithMessage("Invalid chat id.");

            RuleFor(command => command.MessageText)
                .NotEmpty().WithMessage("Message cannot be empty");
        }

        private async Task<bool> ChatExists(int chatId)
        {
            return await _chatRepository.ExistsAsync(chatId);
        }      
    }
}
