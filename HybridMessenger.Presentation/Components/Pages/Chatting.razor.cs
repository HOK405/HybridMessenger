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
        public string ChatId { get; set; }

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

            _requestModel = new ChatMessagesRequest
            {
                PageNumber = 1,
                PageSize = 100,
                SortBy = "SentAt",
                SearchValue = "",
                Ascending = true,
                Fields = new List<string>()
            };

            if (!string.IsNullOrEmpty(ChatId))
            {
                _requestModel.ChatId = ChatId;
                await _chatService.JoinGroup(_requestModel.ChatId);
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
                await _chatService.LeaveGroup(_requestModel.ChatId);
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
