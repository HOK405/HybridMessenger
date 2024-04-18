using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace HybridMessenger.Presentation.Services
{
    public class ChatService
    {
        public event Action OnMessageReceived;

        private IHttpService _httpService;
        private HubConnection _hubConnection;
        private string _url;

        public ChatService(IConfiguration configuration, IHttpService httpServie)
        {
            _url = configuration.GetValue<string>("HubEndpoint");
            _httpService = httpServie;
        }
        public async Task InitializeAsync()
        {
            string token = await _httpService.GetToken();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnMessageReceived?.Invoke();
            });

            await _hubConnection.StartAsync();
        }
    }
}
