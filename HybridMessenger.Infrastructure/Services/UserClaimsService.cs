using HybridMessenger.Domain.Services;
using System.Security.Claims;

namespace HybridMessenger.Infrastructure.Services
{
    public class UserClaimsService : IUserClaimsService
    {
        public string GetUserId(ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
