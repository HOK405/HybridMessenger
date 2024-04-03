using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Domain.Entities.User>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entities.User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.Repository<Domain.Entities.User>() as IUserRepository;

            if (userRepository == null)
            {
                throw new InvalidOperationException("Repository is not of type IUserRepository");
            }

            var userResult = await userRepository.GetByIdAsync(request.Id);

            return userResult;
        }
    }
}
