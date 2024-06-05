using HybridMessenger.Domain.Services;
using System.Security.Claims;

namespace HybridMessenger.Infrastructure.Services
{
    public class UserClaimsService : IUserClaimsService
    {
        public int GetUserId(ClaimsPrincipal principal)
        {
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
            {
                return userId;
            }
            throw new InvalidOperationException("Invalid user ID claim.");
        }
    }
}
