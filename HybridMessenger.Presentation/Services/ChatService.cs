using Microsoft.AspNetCore.SignalR.Client;

namespace HybridMessenger.Presentation.Services
{
    public class ChatService
    {
        private HubConnection _hubConnection;
        public event Action OnMessageReceived;

        public async Task InitializeAsync(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
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
