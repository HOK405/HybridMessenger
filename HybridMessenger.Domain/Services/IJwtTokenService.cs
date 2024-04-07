using HybridMessenger.Domain.Entities;
using System.Security.Claims;

namespace HybridMessenger.Domain.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
