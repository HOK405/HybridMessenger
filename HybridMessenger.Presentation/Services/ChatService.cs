using HybridMessenger.Presentation.ResponseModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace HybridMessenger.Presentation.Services
{
    public class ChatService
    {
        public event Action<MessageResponse> OnMessageReceived;

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

            _hubConnection.On<MessageResponse>("ReceiveMessage", (message) =>
            {
                OnMessageReceived?.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(string chatId, string message)
        {
            await _hubConnection.SendAsync("SendMessage", chatId, message);
        }

        public async Task JoinGroup(string groupId)
        {
            await _hubConnection.SendAsync("JoinGroup", groupId.ToString());
        }

        public async Task LeaveGroup(string groupId)
        {
            await _hubConnection.SendAsync("LeaveGroup", groupId.ToString());
        }
    }
}
