using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

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
        private string _groupNameToCreate;

        private string _newPrivateChatUserName;

        private string _groupNameToUpdate;
        private string _groupIdToUpdate;

        private string _userNameToAddToGroup;
        private string _groupIdToAddMember;

        private string _chatIdToDelete;

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

        private async Task CreateGroup()
        {
            if (!string.IsNullOrEmpty(_groupNameToCreate))
            {
                await HttpService.SetAccessToken();
                var result = await HttpService.PostAsync<ResponeChatObject>("api/chat/create-group", new CreateGroupModel()
                {
                    ChatName = _groupNameToCreate
                });

                _groupNameToCreate = "";
                await LoadChats();
            }
        }

        private async Task CreatePrivateChat()
        {
            if (!string.IsNullOrEmpty(_newPrivateChatUserName))
            {
                await HttpService.PostAsync<ResponeChatObject>("api/chat/create-private-chat", new CreatePrivateChatModel()
                {
                    UserNameToCreateWith = _newPrivateChatUserName
                });

                _newPrivateChatUserName = "";
                await LoadChats();
            }
        }

        private async Task ChangeGroupName()
        {
            if (!string.IsNullOrEmpty(_groupNameToUpdate) && !string.IsNullOrEmpty(_groupIdToUpdate))
            {
                await HttpService.PutAsync<ResponeChatObject>("api/chat/change-chat-name", new ChangeGroupNameModel()
                {
                    NewChatName = _groupNameToUpdate,
                    ChatId = _groupIdToUpdate       
                });

                _groupNameToUpdate = "";
                _groupIdToUpdate = "";
                await LoadChats();
            }
        }

        private async Task AddMemberToChat()
        {
            if (!string.IsNullOrEmpty(_userNameToAddToGroup) && !string.IsNullOrEmpty(_groupIdToAddMember))
            {
                await HttpService.PutAsync<StringOkResponse>("api/chat/add-group-member", new AddGroupMemberModel()
                {
                    ChatId = _groupIdToAddMember,
                    UserNameToAdd = _userNameToAddToGroup
                });

                _userNameToAddToGroup = "";
                _groupIdToAddMember = "";
            }
        }

        private async Task DeleteChat()
        {
            if (!string.IsNullOrEmpty(_chatIdToDelete))
            {
                await HttpService.PostAsync<StringOkResponse>("api/chat/delete-chat", new DeleteChatModel()
                {
                    ChatId = _chatIdToDelete
                });

                _chatIdToDelete = "";
                await LoadChats(); 
            }
        }

        private async Task LoadChats()
        {
            _requestModel.Fields = string.IsNullOrEmpty(_fieldsInput) ? new List<string>() : _fieldsInput.Split(',').Select(f => f.Trim()).ToList();

            await HttpService.SetAccessToken();
            _data = await HttpService.PostAsync<IEnumerable<dynamic>>("api/chat/get-my-chats", _requestModel);

            _chatRequestedFields = string.IsNullOrEmpty(_fieldsInput) ? _allChatDtoFields : _requestModel.Fields;
            StateHasChanged();
        }

        public class StringOkResponse
        {
            public string Message { get; set; }
        }

        public class ResponeChatObject
        {
            public Guid ChatId { get; set; }

            public string? ChatName { get; set; }

            public bool IsGroup { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }
}
