using FluentValidation;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;

namespace HybridMessenger.Application.Message.Commands
{
    public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepository;
        public DeleteMessageCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();

            RuleFor(c => c.MessageId)
                .NotEmpty().WithMessage("Message id can't be empty.")
                .MustAsync(MessageExists).WithMessage("The current message doesn't exist.");
        }

        private async Task<bool> MessageExists(int messageId, CancellationToken cancellationToken)
        {
            var result = await _messageRepository.GetByIdAsync(messageId);

            if (result is null)
            {
                return false;
            }

            return true;
        }
    }
}
