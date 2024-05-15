using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Chatting : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IHttpService HttpService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public ChatService _chatService { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<MessageResponse> _data;

        private ChatMessagesPaginationRequest _requestModel;

        [Parameter]
        public string ChatId
        {
            get => _chatId.ToString();
            set
            {
                if (int.TryParse(value, out var parsedId))
                {
                    _chatId = parsedId;
                }
                else
                {
                    throw new ArgumentException("ChatId must be a valid integer");
                }
            }
        }

        private int _chatId;

        private string _messageText;

        private int _userId;

        private bool _disposed = false;

        protected override async Task OnInitializedAsync()
        {
            await _chatService.InitializeAsync();
            _chatService.OnMessageReceived += HandleNewMessage;

            _data = new List<MessageResponse>();

            _requestModel = new ChatMessagesPaginationRequest{ SortBy = "SentAt" };

            if (_chatId != 0)
            {
                _requestModel.ChatId = _chatId;
                await _chatService.JoinChat(ChatId);
                await LoadMessages();
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _userId = int.Parse(authState.User.FindFirst("nameid")?.Value ?? "0");
            await JSRuntime.InvokeVoidAsync("startConnection", ChatId, _chatService.HubAddress, await HttpService.GetToken());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_data != null && _data.Any())
            {
                await JSRuntime.InvokeVoidAsync("scrollToBottom", "messageContainer");
            }
        }

        public async Task StartCall()
        {
            await _chatService.StartCall(ChatId);
        }

        public async Task EndCall()
        {
            await JSRuntime.InvokeVoidAsync("endCall");
        }

        private void HandleNewMessage(MessageResponse message)
        {
            InvokeAsync(() =>
            {
                _data.Add(message);
                StateHasChanged();
            });
        }

        private async Task LoadMessages()
        {
            _data = await HttpService.PostAsync<List<MessageResponse>>("api/Message/get-chat-messages", _requestModel);         

            StateHasChanged();
        }

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(_messageText))
            {
                await _chatService.SendMessage(_requestModel.ChatId, _messageText);
                _messageText = default; 
            }
        }
        public async ValueTask DisposeAsync()
        {
            await EndCall();
            if (!_disposed)
            {
                _chatService.OnMessageReceived -= HandleNewMessage;
                await _chatService.LeaveChat(ChatId);
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
