using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Application.User.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Domain.Entities.User>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;

        public CreateUserCommandHandler(UserManager<Domain.Entities.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Domain.Entities.User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return user;
            }

            // Aggregate errors
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ArgumentException($"User creation failed: {errors}");
        }
    }

}
