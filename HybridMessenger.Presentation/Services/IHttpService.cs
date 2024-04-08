namespace HybridMessenger.Presentation.Services
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string uri);
        Task<T> PostAsync<T>(string uri, object value);
    }
}
