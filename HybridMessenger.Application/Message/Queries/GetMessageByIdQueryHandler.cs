using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, Domain.Entities.Message>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMessageByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entities.Message> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var messageRepository = _unitOfWork.Repository<Domain.Entities.Message>() as IMessageRepository;

            if (messageRepository == null)
            {
                throw new InvalidOperationException("Repository is not of type IMessageRepository");
            }

            var message = await messageRepository.GetByIdAsync(request.Id);

            return message;
        }
    }
}
