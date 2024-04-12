using AutoMapper;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class ChangeChatNameCommandHandler : IRequestHandler<ChangeChatNameCommand, ChatDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;

        public ChangeChatNameCommandHandler(IUnitOfWork unitOfOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
            _mapper = mapper;
        }

        public async Task<ChatDto> Handle(ChangeChatNameCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(request.ChatId));

            chat.ChatName = request.NewChatName;

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ChatDto>(chat);
        }
    }
}
