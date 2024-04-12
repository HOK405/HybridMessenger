using AutoMapper;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, ChatDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;
        private readonly IUserRepository _userRepository;

        public CreateGroupCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();
            _userRepository = _unitOfWork.GetRepository<IUserRepository>();
        }

        public async Task<ChatDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            Guid.TryParse(request.UserId, out Guid userGuidId);

            var userToAdd = await _userRepository.GetByIdAsync(userGuidId);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var createdChat = await _chatRepository.CreateChatAsync(request.ChatName, true);
                    await _chatMemberRepository.AddUserToChatAsync(userToAdd, createdChat);

                    await transaction.CommitAsync(cancellationToken);

                    return _mapper.Map<ChatDto>(createdChat);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}
