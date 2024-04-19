using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class PagedChats
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        private IEnumerable<ResponeChatObject> _data;
        private PaginationRequest _requestModel;

        private string _fieldsInput;
        private string _groupNameToCreate;

        private string _newPrivateChatUserName;

        private string _groupNameToUpdate;
        private string _groupIdToUpdate;

        private string _userNameToAddToGroup;
        private string _groupIdToAddMember;

        private string _chatIdToDelete;

        private void RedirectToChatPage(Guid chatId)
        {
            NavigationManager.NavigateTo($"/chatting/{chatId}");
        }

        protected override async Task OnInitializedAsync()
        {
            _data = new List<ResponeChatObject>();

            _requestModel = new PaginationRequest
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
                var result = await HttpService.PostAsync<ResponeChatObject>("api/chat/create-group", new CreateGroupRequest()
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
                await HttpService.PostAsync<ResponeChatObject>("api/chat/create-private-chat", new CreatePrivateChatRequest()
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
                await HttpService.PutAsync<ResponeChatObject>("api/chat/change-chat-name", new ChangeGroupNameRequest()
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
                await HttpService.PutAsync<StringOkResponse>("api/chat/add-group-member", new AddGroupMemberRequest()
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
                await HttpService.PostAsync<StringOkResponse>("api/chat/delete-chat", new DeleteChatRequest()
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
            _data = await HttpService.PostAsync<IEnumerable<ResponeChatObject>>("api/chat/get-my-chats", _requestModel);

            StateHasChanged();
        }
    }
}
