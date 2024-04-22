using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Chatting : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IHttpService HttpService { get; set; }

        [Inject]
        public ChatService _chatService { get; set; }

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

        private List<MessageResponse> _data;

        private ChatMessagesRequest _requestModel;

        private bool _disposed = false;

        private readonly List<string> _allUserDtoFields = new List<string>
        {
            "MessageId", "UserId", "MessageText", "SentAt"
        };

        protected override async Task OnInitializedAsync()
        {
            await _chatService.InitializeAsync();
            _chatService.OnMessageReceived += HandleNewMessage;

            _data = new List<MessageResponse>();

            _requestModel = new ChatMessagesRequest{ SortBy = "SentAt" };

            if (_chatId != 0)
            {
                _requestModel.ChatId = _chatId; 
                await _chatService.JoinChat(_chatId);
                await LoadMessages();
            }
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
            await _chatService.SendMessage(_requestModel.ChatId, _messageText);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _chatService.OnMessageReceived -= HandleNewMessage;
                await _chatService.LeaveChat(_requestModel.ChatId);
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
