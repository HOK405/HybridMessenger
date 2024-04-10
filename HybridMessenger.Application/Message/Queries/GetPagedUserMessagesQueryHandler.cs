using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedUserMessagesQueryHandler : IRequestHandler<GetPagedUserMessagesQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPagedUserMessagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<object>> Handle(GetPagedUserMessagesQuery request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
