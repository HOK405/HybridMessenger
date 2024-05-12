using HybridMessenger.Presentation.ResponseModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace HybridMessenger.Presentation.Services
{
    public class ChatService
    {
        public event Action<MessageResponse> OnMessageReceived;

        private IHttpService _httpService;
        private HubConnection _hubConnection;
        private string _url;

        public ChatService(IConfiguration configuration, IHttpService httpService)
        {
            string baseAddress = configuration.GetValue<string>("ApiBaseAddress");
            string endpoint = configuration.GetValue<string>("HubEndpoint");
            _url = "https://localhost:44314/chathub";
            _httpService = httpService;
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

        public async Task StartCall(string chatId)
        {
            await _hubConnection.InvokeAsync("StartCall", chatId);
        }

        [JSInvokable]
        public async Task SendOffer(string chatId, string offer)
        {
            await _hubConnection.SendAsync("SendOffer", chatId, offer);
        }

        [JSInvokable]
        public async Task SendAnswer(string chatId, string answer)
        {
            await _hubConnection.SendAsync("SendAnswer", chatId, answer);
        }

        [JSInvokable]
        public async Task SendIceCandidate(string chatId, string candidate)
        {
            await _hubConnection.SendAsync("SendIceCandidate", chatId, candidate);
        }

        public async Task JoinChat(string chatId)
        {
            await _hubConnection.SendAsync("JoinChat", chatId);
        }

        public async Task LeaveChat(string chatId)
        {
            await _hubConnection.SendAsync("LeaveChat", chatId);
        }
        public async Task SendMessage(int chatId, string message)
        {
            await _hubConnection.SendAsync("SendMessage", chatId, message);
        }
    }
}
