using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class Chatting : ComponentBase
    {
        [Inject]
        private IHttpService HttpService { get; set; }
        private IEnumerable<dynamic> _data;

        private ChatMessagesRequestModel _requestModel;
        private SendMessageRequestModel _sendModel;

        private readonly List<string> _allUserDtoFields = new List<string>
        {
            "MessageId", "UserId", "MessageText", "SentAt"
        };

        protected override async Task OnInitializedAsync()
        {
            _data = new List<dynamic>();

            _requestModel = new ChatMessagesRequestModel
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "SentAt",
                SearchValue = "",
                Ascending = true,
                Fields = new List<string>()
            };

            _sendModel = new SendMessageRequestModel()
            {
                MessageText = ""
            };
        }

        private async Task LoadMessages()
        {
            await HttpService.SetAccessToken();
            _data = await HttpService.PostAsync<IEnumerable<dynamic>>("api/Message/get-user-messages", _requestModel);         

            StateHasChanged();
        }

        private async Task SendMessage()
        {
            await HttpService.SetAccessToken();
            _sendModel.ChatId = _requestModel.ChatId;
            await HttpService.PostAsync<StringOkResponse>("api/Message/send-message", _sendModel);

            await LoadMessages();
        }

        public class StringOkResponse
        {
            public string Message { get; set; }
        }
    }
}
