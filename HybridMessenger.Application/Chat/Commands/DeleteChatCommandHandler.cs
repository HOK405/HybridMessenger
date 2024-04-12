using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Chat.Commands
{
    public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;

        public DeleteChatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
        }

        public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out Guid userIdGuid))
            {
                throw new ArgumentException("The provided UserId is not a valid GUID.", nameof(request.UserId));
            }

            var chat = await _chatRepository.GetByIdAsync(request.ChatId);

            if (chat == null)
            {
                return false;
            }
            else
            {
                _chatRepository.Remove(chat);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
        }
    }
}
