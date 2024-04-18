using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Chatting : ComponentBase
    {
        [Inject]
        public IHttpService HttpService { get; set; }

        [Inject]
        public ChatService _chatService { get; set; }

        private string _messageText;

        private List<MessageModel> _data;

        private ChatMessagesRequestModel _requestModel = new ChatMessagesRequestModel
        {
            ChatId = "AE859DC5-2892-4300-98A4-2C5B6CB9D1B6",
            PageNumber = 1,
            PageSize = 100,
            SortBy = "SentAt",
            SearchValue = "",
            Ascending = true,
            Fields = new List<string>()
        };

        private readonly List<string> _allUserDtoFields = new List<string>
        {
            "MessageId", "UserId", "MessageText", "SentAt"
        };

        protected override async Task OnInitializedAsync()
        {
            await _chatService.InitializeAsync();
            _chatService.OnMessageReceived += HandleNewMessage;

            _data = new List<MessageModel>();

            await HttpService.SetAccessToken();

            await _chatService.JoinGroup(_requestModel.ChatId);

            await LoadMessages();
        }

        private void HandleNewMessage(MessageModel message)
        {
            InvokeAsync(() =>
            {
                _data.Add(message);
                StateHasChanged();
            });
        }

        private async Task LoadMessages()
        {
            _data = await HttpService.PostAsync<List<MessageModel>>("api/Message/get-chat-messages", _requestModel);         

            StateHasChanged();
        }

        private async Task SendMessage()
        {
            await _chatService.SendMessage(_requestModel.ChatId, _messageText);
        }

        public async ValueTask DisposeAsync()
        {
            await _chatService.LeaveGroup(_requestModel.ChatId);
        }

        public class StringOkResponse
        {
            public string Message { get; set; }
        }
    }
}
