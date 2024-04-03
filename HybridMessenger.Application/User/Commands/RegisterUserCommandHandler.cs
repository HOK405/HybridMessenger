using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string> 
    {
        private readonly IUserIdentityService _userService;
        private readonly IJwtTokenService _jwtTokenService; 

        public RegisterUserCommandHandler(IUserIdentityService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User
            {
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = request.PhoneNumber
            };

            await _userService.CreateUserAsync(user, request.Password);

            var token = _jwtTokenService.GenerateToken(user);

            return token;
        }
    }
}
