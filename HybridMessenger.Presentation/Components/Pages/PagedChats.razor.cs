using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using System.Dynamic;
using System.Text.Json;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class PagedChats
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        private IEnumerable<dynamic> _data;

        private List<string> _chatRequestedFields;

        private PaginationRequestModel _requestModel;
        private string _fieldsInput;

        private readonly List<string> _allChatDtoFields = new List<string>
        {
            "ChatId", "ChatName", "IsGroup", "CreatedAt"
        };

        protected override async Task OnInitializedAsync()
        {
            _data = new List<dynamic>();
            _chatRequestedFields = new List<string>();

            _requestModel = new PaginationRequestModel
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "CreatedAt",
                SearchValue = "",
                Ascending = true,
                Fields = new List<string>()
            };

            await LoadChats();
        }

        private async Task LoadChats()
        {
            _requestModel.Fields = string.IsNullOrEmpty(_fieldsInput) ? new List<string>() : _fieldsInput.Split(',').Select(f => f.Trim()).ToList();

            await HttpService.SetAccessToken();
            _data = await HttpService.PostAsync<IEnumerable<dynamic>>("api/chat/get-my-chats", _requestModel);

            _chatRequestedFields = string.IsNullOrEmpty(_fieldsInput) ? _allChatDtoFields : _requestModel.Fields;
            StateHasChanged();
        }
    }
}
