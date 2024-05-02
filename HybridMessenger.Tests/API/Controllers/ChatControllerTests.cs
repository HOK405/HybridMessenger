using HybridMessenger.API;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class ChatControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private List<int> _createdChatIds = new List<int>();

        public ChatControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/"));
        }

        public async Task InitializeAsync()
        {
            _accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task DisposeAsync()
        {
            await Task.WhenAll(_createdChatIds.Select(DeleteChat));
        }


        [Fact]
        public async Task CreateGroup_ValidRequest_ReturnsOk()
        {
            // Arrange & Act
            var newGroup = await CreatePublicGroupAsync("New Group");

            // Assert
            Assert.NotNull(newGroup);
            Assert.True(newGroup.ChatId > 0);
            Assert.Equal("New Group", newGroup.ChatName);
        }


        [Fact]
        public async Task CreatePrivateChat_ValidRequest_ReturnsOk()
        {
            // Arrange & Act
            var newPrivateChat = await CreatePrivateChatAsync("userToChatWith123");

            // Assert
            Assert.NotNull(newPrivateChat);
            Assert.True(newPrivateChat.ChatId > 0);
        }


        [Fact]
        public async Task GetUserChats_ValidRequest_ReturnsOk()
        {
            // Arrange
            var query = DefaultChatData.GetPagedUserChatsQuery();

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/get-my-chats", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var chats = JsonConvert.DeserializeObject<List<ChatResponseModel>>(result);

            // Assert
            Assert.NotNull(chats);
            Assert.Equal(2, chats.Count);  
            Assert.Contains(chats, chat => chat.ChatName == "Group for testing"); 
            Assert.Contains(chats, chat => chat.ChatName == "Group for testing 2"); 
        }


        [Fact]
        public async Task AddGroupMemberByUsername_ValidRequest_ReturnsOk()
        {
            // Arrange
            var newGroup = await CreatePublicGroupAsync("Group for Adding Members");
            var command = new AddGroupMemberCommand { ChatId = newGroup.ChatId, UserNameToAdd = "userToChatWith123" };

            // Act
            var response = await _httpClient.PutAsJsonAsync("chat/add-group-member", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageTextResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("successfully added", result.Message);
        }


        [Fact]
        public async Task ChangeChatName_ValidRequest_ReturnsOk()
        {
            // Arrange
            var newGroup = await CreatePublicGroupAsync("Group for Name Change");

            string newGroupName = "Updated Group Name";
            var command = new ChangeGroupNameCommand
            {
                ChatId = newGroup.ChatId,
                NewChatName = newGroupName
            };

            // Act
            var response = await _httpClient.PutAsJsonAsync("chat/change-chat-name", command);
            response.EnsureSuccessStatusCode();
            var contentResult = await response.Content.ReadAsStringAsync();
            var updatedGroup = JsonConvert.DeserializeObject<ChatResponseModel>(contentResult);

            // Assert
            Assert.NotNull(updatedGroup);
            Assert.Equal(newGroupName, updatedGroup.ChatName); 
        }


        [Fact]
        public async Task DeletePublicGroup_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            var publicChat = await CreatePublicGroupAsync("New Public Group");
            var command = new DeleteChatCommand { ChatId = publicChat.ChatId };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageTextResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Chat is successfully deleted.", result.Message);
        }


        [Fact]
        public async Task DeletePrivateChat_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            var privateChat = await CreatePrivateChatAsync("userToChatWith123");
            var command = new DeleteChatCommand { ChatId = privateChat.ChatId };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageTextResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Chat is successfully deleted.", result.Message);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var loginCommand = DefaultUserData.GetLoginCommand();

            var response = await _httpClient.PostAsJsonAsync("user/login", loginCommand);
            response.EnsureSuccessStatusCode();
            var loginResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(await response.Content.ReadAsStringAsync());
            return loginResult.AccessToken;
        }


        private async Task DeleteChat(int chatId)
        {
            var command = new DeleteChatCommand { ChatId = chatId };
            await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
        }

        private async Task<ChatResponseModel> CreatePublicGroupAsync(string groupName)
        {
            var command = new CreateGroupCommand { ChatName = groupName };
            var response = await _httpClient.PostAsJsonAsync("chat/create-group", command);
            response.EnsureSuccessStatusCode();
            var chatResult = JsonConvert.DeserializeObject<ChatResponseModel>(await response.Content.ReadAsStringAsync());
            _createdChatIds.Add(chatResult.ChatId);
            return chatResult;
        }

        private async Task<ChatResponseModel> CreatePrivateChatAsync(string username)
        {
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = username };
            var response = await _httpClient.PostAsJsonAsync("chat/create-private-chat", command);
            response.EnsureSuccessStatusCode();
            var chatResult = JsonConvert.DeserializeObject<ChatResponseModel>(await response.Content.ReadAsStringAsync());
            _createdChatIds.Add(chatResult.ChatId);
            return chatResult;
        }
    }
}
