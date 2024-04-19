using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class PagedMessages
    {
        [Inject]
        private IHttpService HttpService { get; set; }
        private IEnumerable<dynamic> _data;
        private List<string> _userRequestedFields;

        private PaginationRequest _requestModel;

        private string _fieldsInput;

        private readonly List<string> _allUserDtoFields = new List<string>
        {
            "MessageId", "ChatId", "UserId", "MessageText", "SentAt"
        };

        protected override async Task OnInitializedAsync()
        {
            _data = new List<dynamic>();
            _userRequestedFields = new List<string>();

            _requestModel = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "SentAt",
                SearchValue = "",
                Ascending = true,
                Fields = new List<string>()
            };

            await LoadMessages();
        }

        private async Task LoadMessages()
        {
            _requestModel.Fields = string.IsNullOrEmpty(_fieldsInput) ? new List<string>() : _fieldsInput.Split(',').Select(f => f.Trim()).ToList();

            await HttpService.SetAccessToken();
            _data = await HttpService.PostAsync<IEnumerable<dynamic>>("api/Message/get-user-messages", _requestModel);

            _userRequestedFields = string.IsNullOrEmpty(_fieldsInput) ? _allUserDtoFields : _requestModel.Fields;

            StateHasChanged();
        }
    }
}
