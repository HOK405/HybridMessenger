using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Commands
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepository;

        public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();
        }
        public async Task Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
        {
            if (!await _messageRepository.IsUserMessageAsync(command.MessageId, command.UserId))
            {
                throw new ArgumentException("The message can be deleted only by its sender.");
            }

            var message = await _messageRepository.GetByIdAsync(command.MessageId);
            _messageRepository.Remove(message);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
