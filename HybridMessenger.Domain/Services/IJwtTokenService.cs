using HybridMessenger.Domain.Entities;

namespace HybridMessenger.Domain.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
