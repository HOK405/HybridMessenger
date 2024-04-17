using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        }

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

        public async Task SetAccessToken()
        {
            string accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
