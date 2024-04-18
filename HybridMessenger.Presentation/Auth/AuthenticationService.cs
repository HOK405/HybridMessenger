using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HybridMessenger.Presentation.Auth
{
    public class AuthenticationService : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly static string _accessTokenKey = "accessToken";

        public AuthenticationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _accessTokenKey);

            var identity = new ClaimsIdentity();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(accessToken);
                    var claims = token.Claims;
                    identity = new ClaimsIdentity(claims, "jwt");
                }
                catch
                {
                    throw new ArgumentException();  // change it in the future
                }
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var identity = new ClaimsIdentity(token.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
