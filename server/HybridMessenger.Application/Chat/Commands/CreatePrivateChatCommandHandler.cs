using AutoMapper;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class CreatePrivateChatCommandHandler : IRequestHandler<CreatePrivateChatCommand, ChatDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatMemberRepository _chatMemberRepository;

        public CreatePrivateChatCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _userRepository = _unitOfWork.GetRepository<IUserRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();
        }

        public async Task<ChatDto> Handle(CreatePrivateChatCommand command, CancellationToken cancellationToken)
        {
            var userToCreateWith = await _userRepository.GetUserByUsernameAsync(command.UserNameToCreateWith);

            var userCreator = await _userRepository.GetByIdAsync(command.UserCreatorId);

            var createdChat = await _chatRepository.CreateChatAsync(null, false);

            await _chatMemberRepository.AddUserToChatAsync(userCreator, createdChat);
            await _chatMemberRepository.AddUserToChatAsync(userToCreateWith, createdChat);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ChatDto>(createdChat);
        }
    }
}
