using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class PagedChats : ComponentBase
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JS {  get; set; }


        private IEnumerable<ResponeChatObject> _data;
        private PaginationRequest _requestModel;

        private string _fieldsInput;
        private string _groupNameToCreate;

        private string _newPrivateChatUserName;

        private string _groupNameToUpdate;
        private string _groupIdToUpdate;

        private string _userNameToAddToGroup;
        private string _groupIdToAddMember;

        private bool _editMode;

        private ResponeChatObject _selectedChat;

        protected override async Task OnInitializedAsync()
        {
            _data = new List<ResponeChatObject>();

            _requestModel = new PaginationRequest { SortBy = "CreatedAt", PageSize = 5 };

            await LoadChats();
        }

        private void ShowEditOptions(ResponeChatObject chat)
        {
            _selectedChat = chat;
            _groupNameToUpdate = chat.ChatName;
            _groupIdToUpdate = chat.ChatId.ToString();
            _groupIdToAddMember = chat.ChatId.ToString();
            _editMode = true;
        }

        private async Task CloseAndRefresh()
        {
            _selectedChat = null;
            _editMode = false;
            await LoadChats();
        }

        private void RedirectToChatPage(int chatId)
        {
            NavigationManager.NavigateTo($"/chatting/{chatId}");
        }

        private async Task CreateGroup()
        {
            if (!string.IsNullOrEmpty(_groupNameToCreate))
            {
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
            if (!string.IsNullOrEmpty(_selectedChat.ChatId.ToString()))
            {
                await HttpService.PostAsync<StringOkResponse>("api/chat/delete-chat", new DeleteChatRequest()
                {
                    ChatId = _selectedChat.ChatId.ToString()
                });

                _selectedChat.ChatId = default;
                await CloseAndRefresh();
            }
        }

        private async Task DeleteChatById(int chatId)
        {
            await HttpService.PostAsync<StringOkResponse>("api/chat/delete-chat", new DeleteChatRequest()
            {
                ChatId = chatId.ToString()
            });

            await LoadChats(); 
        }


        private async Task LoadChats()
        {
            _requestModel.Fields = string.IsNullOrEmpty(_fieldsInput) ? new List<string>() : _fieldsInput.Split(',').Select(f => f.Trim()).ToList();

            _data = await HttpService.PostAsync<IEnumerable<ResponeChatObject>>("api/chat/get-my-chats", _requestModel);

            StateHasChanged();
        }

        private async Task NextPage()
        {
            _requestModel.PageNumber++;
            await LoadChats();
        }

        private async Task PreviousPage()
        {
            if (_requestModel.PageNumber > 1)
            {
                _requestModel.PageNumber--;
                await LoadChats();
            }
        }
    }
}
