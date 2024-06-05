using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HybridMessenger.Presentation.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        public HttpService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            EnsureAccessToken();
        }

        #region HTTP Methods

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostAsync<T>(string uri, object value)
        {
            var response = await _httpClient.PostAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostFileAsync<T>(string uri, MultipartFormDataContent content)
        {
            var response = await _httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PutAsync<T>(string uri, object value)
        {
            var response = await _httpClient.PutAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task DeleteAsync(string uri)
        {
            var response = await _httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        public async Task SetTokens(string accessToken, string refreshToken)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
            UpdateHttpClientAuthorizationHeader(accessToken);
        }

        public async Task ClearTokens()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<string> GetToken()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        }

        private async Task EnsureAccessToken()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var token = await GetToken();
                UpdateHttpClientAuthorizationHeader(token);
            }
        }

        private void UpdateHttpClientAuthorizationHeader(string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
