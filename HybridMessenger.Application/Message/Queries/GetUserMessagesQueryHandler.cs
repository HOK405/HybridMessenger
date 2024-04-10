using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetUserMessagesQueryHandler : IRequestHandler<GetUserMessagesQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserMessagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<object>> Handle(GetUserMessagesQuery request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
