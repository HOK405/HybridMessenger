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
    public class ChatControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

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
                Email = "firstUserTest123@mail.com",
                Password = "firstUserTest123"
            };

            var response = await _httpClient.PostAsJsonAsync("user/login", loginCommand);
            response.EnsureSuccessStatusCode();
            var loginResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(await response.Content.ReadAsStringAsync());
            return loginResult.AccessToken;
        }

        [Fact]
        public async Task CreateGroup_ValidRequest_ReturnsOk()
        {
            // Arrange
            var command = new CreateGroupCommand { ChatName = "New Group" };
            
            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/create-group", command);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
        }


        [Fact]
        public async Task CreatePrivateChat_ValidRequest_ReturnsOk()
        {
            // Arrange
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = "newUserTest123" };

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/create-private-chat", command);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
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
                Ascending = true, 
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
        public async Task ChangeChatName_ValidRequest_ReturnsOk()
        {
            // Arrange
            var command = new ChangeGroupNameCommand 
            { 
                ChatId = 5, 
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
        public async Task AddGroupMemberByUsername_ValidRequest_ReturnsOk()
        {
            // Arrange
            var command = new AddGroupMemberCommand { ChatId = 4, UserNameToAdd = "userf728322ff7654106a59c8e1236be3470" };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            // Act
            var response = await _httpClient.PutAsJsonAsync("chat/add-group-member", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("successfully added", result.Message);
        }

        [Fact]
        public async Task DeleteChat_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            var command = new DeleteChatCommand { ChatId = 13 }; 

            // Act
            var response = await _httpClient.PostAsJsonAsync("chat/delete-chat", command);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Chat is successfully deleted.", result.Message);
        }
    }
}
