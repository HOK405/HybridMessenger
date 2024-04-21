using System.Security.Claims;

namespace HybridMessenger.Domain.Services
{
    public interface IUserClaimsService
    {
        int GetUserId(ClaimsPrincipal principal);
    }
}
