﻿using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private LoginModel loginModel = new LoginModel();
        private string loginResult;
        private string alertClass => loginResult == "Logged in successfully!" ? "status-success" : "status-danger";

        private async Task HandleLogin()
        {
            try
            {
                var tokenResponse = await HttpService.PostAsync<TokenResponse>("api/User/login", loginModel);
                if (tokenResponse != null && !string.IsNullOrWhiteSpace(tokenResponse.Token))
                {
                    await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.Token);
                    loginResult = "Logged in successfully!";
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
