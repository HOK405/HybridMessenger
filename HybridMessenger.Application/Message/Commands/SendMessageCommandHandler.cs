using AutoMapper;
using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Commands
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepository;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();
            _mapper = mapper;
        }

        public async Task<MessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var newMessge = new Domain.Entities.Message()
            {
                MessageText = command.MessageText,
                ChatId = command.ChatId,
                UserId = command.UserId,
                SentAt = DateTime.UtcNow
            };

            var messageEntity = await _messageRepository.AddAsync(newMessge);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MessageDto>(messageEntity);
        }
    }
}
