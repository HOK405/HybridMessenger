using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetEmailByUsernameQueryHandler : IRequestHandler<GetEmailByUsernameQuery, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEmailByUsernameQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(GetEmailByUsernameQuery request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.GetRepository<IUserRepository>();

            string emailResult = await userRepository.FindEmailByUsernameAsync(request.Username);

            return emailResult;
        }
    }
}
