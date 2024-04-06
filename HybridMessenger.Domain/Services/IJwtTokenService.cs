using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken(User user);
    }
}
