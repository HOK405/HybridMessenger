using System.Security.Claims;

namespace HybridMessenger.Domain.Services
{
    public interface IUserClaimsService
    {
        string GetUserId(ClaimsPrincipal principal);
    }
}
