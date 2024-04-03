using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.Application.User.Commands
{
    public class VerifyByEmailPasswordCommandHandler : IRequestHandler<VerifyByEmailPasswordCommand, Domain.Entities.User>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;

        public VerifyByEmailPasswordCommandHandler(UserManager<Domain.Entities.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Domain.Entities.User> Handle(VerifyByEmailPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                throw new ArgumentException("Invalid password.");
            }

            return user; 
        }
    }
}
