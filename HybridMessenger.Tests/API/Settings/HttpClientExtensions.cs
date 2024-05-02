using Newtonsoft.Json;

namespace HybridMessenger.Tests.API.Settings
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostAsJsonAsync<T>(this HttpClient httpClient, string url, object data)
        {
            var response = await httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<T> PutAsJsonAsync<T>(this HttpClient httpClient, string url, object data)
        {
            var response = await httpClient.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
