using HybridMessenger.API;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.Queries;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class ChatControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private List<int> _createdChatIds = new List<int>();

        public ChatControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/"));

            _accessToken = GetAccessTokenAsync().GetAwaiter().GetResult();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var loginCommand = new VerifyByEmailPasswordCommand
            {
                Email = "testUser999@mail.com",
                Password = "testUser999"
            };

            var response = await _httpClient.PostAsJsonAsync("user/login", loginCommand);
            response.EnsureSuccessStatusCode();
            var loginResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(await response.Content.ReadAsStringAsync());
            return loginResult.AccessToken;
        }

        [Fact]
        public async Task CreateGroup_ValidRequest_ReturnsOk()
        {
            // Arrange & Act
            int newGroupId = await CreatePublicGroupAsync("New Group");

            // Assert
            Assert.True(newGroupId > 0); 
        }


        [Fact]
        public async Task CreatePrivateChat_ValidRequest_ReturnsOk()
        {
            // Arrange & Act
            int newPrivateChatId = await CreatePrivateChatAsync("userToChatWith123");

            // Assert
            Assert.True(newPrivateChatId > 0); 
        }


        [Fact]
        public async Task GetUserChats_ValidRequest_ReturnsOk()
        {
            // Arrange
            var query = new GetPagedUserChatsQuery 
            { 
                PageNumber = 1, 
                PageSize = 10, 
                SortBy = "CreatedAt", 
                SearchValue = "", 
                Ascending = false, 
                Fields = { } 
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/get-my-chats", query);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
        }


        [Fact]
        public async Task AddGroupMemberByUsername_ValidRequest_ReturnsOk()
        {
            // Arrange
            int newGroupId = await CreatePublicGroupAsync("Group for Adding Members");
            var command = new AddGroupMemberCommand { ChatId = newGroupId, UserNameToAdd = "userToChatWith123" };

            // Act
            var response = await _httpClient.PutAsJsonAsync("chat/add-group-member", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("successfully added", result.Message);
        }


        [Fact]
        public async Task ChangeChatName_ValidRequest_ReturnsOk()
        {
            // Arrange
            int newGroupId = await CreatePublicGroupAsync("Group for Name Change");
            var command = new ChangeGroupNameCommand
            {
                ChatId = newGroupId,
                NewChatName = "Updated Group Name"
            };

            // Act
            var response = await _httpClient.PutAsJsonAsync("chat/change-chat-name", command);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
        }


        [Fact]
        public async Task DeletePublicGroup_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            int publicChatId = await CreatePublicGroupAsync("New Public Group");

            var command = new DeleteChatCommand { ChatId = publicChatId };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Chat is successfully deleted.", result.Message);
        }


        [Fact]
        public async Task DeletePrivateChat_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            int privateChatId = await CreatePrivateChatAsync("userToChatWith123");

            var command = new DeleteChatCommand { ChatId = privateChatId };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Chat is successfully deleted.", result.Message);
        }


        public void Dispose()
        {
            foreach (var chatId in _createdChatIds)
            {
                DeleteChat(chatId).Wait(); 
            }
        }

        private async Task DeleteChat(int chatId)
        {
            var command = new DeleteChatCommand { ChatId = chatId };
            await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
        }

        private async Task<int> CreatePublicGroupAsync(string groupName)
        {
            var command = new CreateGroupCommand { ChatName = groupName };
            var response = await _httpClient.PostAsJsonAsync("chat/create-group", command);
            response.EnsureSuccessStatusCode();
            var chatResult = JsonConvert.DeserializeObject<ChatResponseModel>(await response.Content.ReadAsStringAsync());
            _createdChatIds.Add(chatResult.ChatId);
            return chatResult.ChatId;
        }

        private async Task<int> CreatePrivateChatAsync(string username)
        {
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = username };
            var response = await _httpClient.PostAsJsonAsync("chat/create-private-chat", command);
            response.EnsureSuccessStatusCode();
            var chatResult = JsonConvert.DeserializeObject<ChatResponseModel>(await response.Content.ReadAsStringAsync());
            _createdChatIds.Add(chatResult.ChatId);
            return chatResult.ChatId;
        }
    }
}
