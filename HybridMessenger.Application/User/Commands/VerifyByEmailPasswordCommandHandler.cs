using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class VerifyByEmailPasswordCommandHandler : IRequestHandler<VerifyByEmailPasswordCommand, (string, string)>
    {
        private readonly IUserIdentityService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public VerifyByEmailPasswordCommandHandler(IUserIdentityService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(string, string)> Handle(VerifyByEmailPasswordCommand request, CancellationToken cancellationToken)
        {
            var user =  await _userService.VerifyUserByEmailAndPasswordAsync(request.Email, request.Password);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return (await _jwtTokenService.GenerateAccessToken(user), await _jwtTokenService.GenerateRefreshToken(user));
        }
    }
}