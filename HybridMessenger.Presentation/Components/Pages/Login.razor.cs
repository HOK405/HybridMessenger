using HybridMessenger.Presentation.Auth;
using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Login
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        private IHttpService _httpService { get; set; }
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        private LoginModel loginModel = new LoginModel();
        private string loginResult;
        private string alertClass => loginResult == "Logged in successfully!" ? "status-success" : "status-danger";

        private async Task HandleLogin()
        {
            try
            {
                var tokenResponse = await _httpService.PostAsync<TokenResponse>("api/User/login", loginModel);
                if (tokenResponse != null && !string.IsNullOrWhiteSpace(tokenResponse.AccessToken) && !string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                {
                    await _httpService.SetAccessToken();
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResponse.AccessToken);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", tokenResponse.RefreshToken);

                    loginResult = "Logged in successfully!";
                    ((CustomAuthStateProvider)AuthenticationStateProvider).NotifyUserAuthentication(tokenResponse.AccessToken);
                    NavigationManager.NavigateTo("/users", true);
                }
                else
                {
                    loginResult = "You aren't logged in. Incorrect email or password.";
                }
            }
            catch (HttpRequestException ex)
            {
                loginResult = $"Login failed: {ex.Message}";
            }
            catch (NotSupportedException)
            {
                loginResult = "Error: The content type is not supported.";
            }
            catch (JsonException)
            {
                loginResult = "Error: Problem with JSON data.";
            }       
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
