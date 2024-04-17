using HybridMessenger.Presentation.Models;
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

        private IEnumerable<dynamic> _data;

        private ChatMessagesRequestModel _requestModel = new ChatMessagesRequestModel
        {
            ChatId = "default",
            PageNumber = 1,
            PageSize = 20,
            SortBy = "SentAt",
            SearchValue = "",
            Ascending = true,
            Fields = new List<string>()  
        };

        private SendMessageRequestModel _sendModel = new SendMessageRequestModel()
        {
            MessageText = ""
        };

        private readonly List<string> _allUserDtoFields = new List<string>
        {
            "MessageId", "UserId", "MessageText", "SentAt"
        };

        protected override async Task OnInitializedAsync()
        {
            await _chatService.InitializeAsync("https://localhost:44314/chathub");
            _chatService.OnMessageReceived += HandleNewMessage;

            _data = new List<dynamic>();

            await HttpService.SetAccessToken();
        }

        private void HandleNewMessage()
        {
            InvokeAsync(async () =>
            {
                await LoadMessages();
            });
        }

        private async Task LoadMessages()
        {
            _data = await HttpService.PostAsync<IEnumerable<dynamic>>("api/Message/get-chat-messages", _requestModel);         

            StateHasChanged();
        }

        private async Task SendMessage()
        {
            _sendModel.ChatId = _requestModel.ChatId;
            await HttpService.PostAsync<StringOkResponse>("api/Message/send-message", _sendModel);
        }

        public class StringOkResponse
        {
            public string Message { get; set; }
        }
    }
}
