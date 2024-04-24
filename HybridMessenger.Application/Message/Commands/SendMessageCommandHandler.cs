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
        private readonly IUserRepository _userRepository;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();
            _userRepository = _unitOfWork.GetRepository<IUserRepository>();
            _mapper = mapper;
        }

        public async Task<MessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByIdAsync(command.UserId);

            if (userResult is null)
            {
                throw new ArgumentNullException(nameof(userResult), "User doesn't exist in database.");
            }

            var newMessge = new Domain.Entities.Message()
            {
                MessageText = command.MessageText,
                ChatId = command.ChatId,
                SentAt = DateTime.UtcNow,
                User = userResult,
            };

            var messageEntity = await _messageRepository.AddAsync(newMessge);
            await _unitOfWork.SaveChangesAsync();

            var username = messageEntity.User?.UserName ?? "Unknown";  

            var messageDto = _mapper.Map<MessageDto>(messageEntity);
            messageDto.SenderUserName = username;  

            return messageDto;
        }
    }
}
