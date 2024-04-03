using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Domain.Entities.User>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;

        public GetUserByIdQueryHandler(UserManager<Domain.Entities.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Domain.Entities.User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // Convert Guid ID to string
            var userIdAsString = request.Id.ToString();

            var user = await _userManager.FindByIdAsync(userIdAsString);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.Id} was not found.");
            }

            return user;
        }
    }
}
