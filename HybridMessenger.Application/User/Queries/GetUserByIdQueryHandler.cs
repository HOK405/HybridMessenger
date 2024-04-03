using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Domain.Entities.User>
    {
        private readonly IUserIdentityService _userService;

        public GetUserByIdQueryHandler(IUserIdentityService userService)
        {
            _userService = userService;
        }

        public async Task<Domain.Entities.User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserByIdAsync(request.Id);
        }
    }
}
