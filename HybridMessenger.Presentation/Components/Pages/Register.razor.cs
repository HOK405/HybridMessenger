using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Register
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IHttpService HttpService { get; set; }

        private RegisterModel registerModel = new RegisterModel();
        private string registerResult;
        private string alertClass => registerResult == "Registered successfully!" ? "status-success" : "status-danger";

        private async Task HandleRegister()
        {
            try
            {
                var tokenResponse = await HttpService.PostAsync<TokenResponse>("api/User/register", registerModel);
                if (tokenResponse != null && !string.IsNullOrWhiteSpace(tokenResponse.Token))
                {
                    await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.Token);
                    registerResult = "Registered successfully!";
                }
                else
                {
                    registerResult = "Registration failed: The server responded with an unexpected status.";
                }
            }
            catch (HttpRequestException ex)
            {
                registerResult = $"Registration failed: {ex.Message}";
            }
            catch (NotSupportedException)
            {
                registerResult = "Registration failed: Unsupported content type.";
            }
            catch (JsonException)
            {
                registerResult = "Registration failed: Invalid JSON data.";
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }

        private class ErrorResponse
        {
            public string Error { get; set; }
        }
    }
}
