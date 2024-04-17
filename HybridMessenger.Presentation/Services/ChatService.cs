using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace HybridMessenger.Presentation.Services
{
    public class ChatService
    {
        public event Action OnMessageReceived;

        private HubConnection _hubConnection;
        private string _url;

        public ChatService(IConfiguration configuration)
        {
            _url = configuration.GetValue<string>("HubEndpoint");
        }
        public async Task InitializeAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_url)
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
