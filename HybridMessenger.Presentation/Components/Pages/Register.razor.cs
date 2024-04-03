using HybridMessenger.Presentation.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Register
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private HttpClient Http { get; set; }

        private RegisterModel registerModel = new RegisterModel();
        private string registerResult;
        private string alertClass => registerResult == "Registered successfully!" ? "status-success" : "status-danger";

        private async Task HandleRegister()
        {
            var response = await Http.PostAsJsonAsync("api/User/register", registerModel);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.Token);
                registerResult = "Registered successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                registerResult = $"Registration failed: {errorResponse.Error}";
            }
            else
            {
                registerResult = "An unexpected error occurred during registration.";
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
