namespace HybridMessenger.Presentation.Services
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string uri);

        Task<T> PostAsync<T>(string uri, object value);

        Task<T> PutAsync<T>(string uri, object value);

        Task DeleteAsync(string uri);

        Task SetAccessToken();
    }
}
