namespace HybridMessenger.Presentation.Services
{
    public interface IHttpService
    {
        Task SetTokens(string accessToken, string refreshToken);

        Task ClearTokens();

        Task<T> GetAsync<T>(string uri);

        Task<T> PostAsync<T>(string uri, object value);

        Task<T> PostFileAsync<T>(string uri, MultipartFormDataContent content);

        Task<T> PutAsync<T>(string uri, object value);

        Task DeleteAsync(string uri);

        Task<string> GetToken();
    }
}
