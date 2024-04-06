using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (string,string)> 
    {
        private readonly IUserIdentityService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        const string _defaultUserRole = "Default";

        public RegisterUserCommandHandler(IUserIdentityService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(string, string)> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User
            {
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = request.PhoneNumber
            };

            await _userService.CreateUserAsync(user, request.Password);

            await _userService.AddRoleAsync(user, _defaultUserRole);

            var accessToken = await _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshToken(user);

            return (accessToken, refreshToken);
        }
    }
}
