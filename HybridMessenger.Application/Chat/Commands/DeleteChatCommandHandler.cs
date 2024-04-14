using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;

        public DeleteChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
        }

        public async Task Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(Guid.Parse(request.ChatId));

            _chatRepository.Remove(chat);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
