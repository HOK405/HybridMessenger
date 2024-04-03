using HybridMessenger.Presentation.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private HttpClient Http { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private LoginModel loginModel = new LoginModel();
        private string loginResult;
        private string alertClass => loginResult == "Logged in successfully!" ? "status-success" : "status-danger";

        private async Task HandleLogin()
        {
            var response = await Http.PostAsJsonAsync("api/User/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.Token);
                loginResult = "Logged in successfully!";
                // Redirect???????????????????
            }
            else
            {
                loginResult = "You aren't logged in. Incorrect email or password.";
            }
        }

        private async Task DeleteJwtFromLocalStorage()
        {
            await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            loginResult = "Logged out successfully.";
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
