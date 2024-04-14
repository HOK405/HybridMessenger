using AutoMapper;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeGroupNameCommandHandler : IRequestHandler<ChangeGroupNameCommand, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;

        public ChangeGroupNameCommandHandler(IUnitOfWork unitOfOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _chatMemberRepository = _unitOfWork.GetRepository<IChatMemberRepository>();
            _mapper = mapper;
        }

        public async Task<ChatDto> Handle(ChangeGroupNameCommand command, CancellationToken cancellationToken)
        {
            if (!await UserIsInChat(command.UserId, command.ChatId))
            {
                throw new InvalidOperationException("User is not a member of the specified chat.");
            }

            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(command.ChatId));

            chat.ChatName = command.NewChatName;

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ChatDto>(chat);
        }

        private async Task<bool> UserIsInChat(string userId, string chatId)
        {
            return await _chatMemberRepository.IsUserMemberOfGroupAsync(Guid.Parse(userId), Guid.Parse(chatId));
        }
    }
}
