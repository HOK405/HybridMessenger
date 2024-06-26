﻿using HybridMessenger.Domain.Services;
using MediatR;
using System.Security.Claims;

namespace HybridMessenger.Application.User.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string>
    {
        private readonly IUserIdentityService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService, IUserIdentityService userService)
        {
            _jwtTokenService = jwtTokenService;
            _userService = userService;
        }

        public async Task<string> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var refreshToken = command.RefreshToken;

            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(refreshToken);
            if (principal == null)
            {
                throw new ArgumentException("Invalid token");
            }

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                throw new ArgumentException("Invalid token - user ID missing or malformed.");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var newAccessToken = await _jwtTokenService.GenerateAccessToken(user);
            return newAccessToken;
        }
    }
}
