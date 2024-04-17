using AutoMapper;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepository;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();
        }

        public async Task Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var newMessge = new Domain.Entities.Message()
            {
                MessageText = command.MessageText,
                ChatID = Guid.Parse(command.ChatId),
                UserID = Guid.Parse(command.UserId),
                SentAt = DateTime.UtcNow
            };

            _messageRepository.AddAsync(newMessge);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
