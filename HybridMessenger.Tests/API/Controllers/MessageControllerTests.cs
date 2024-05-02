using HybridMessenger.API;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class MessageControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private List<int> _createdChatIds = new List<int>();

        public MessageControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/"));         
        }

        private async Task<LoginRegisterResponseModel> AuthenticateUserAsync()
        {
            var loginCommand = new VerifyByEmailPasswordCommand
            {
                Email = "testUser999@mail.com",
                Password = "testUser999"
            };

            var response = await _httpClient.PostAsJsonAsync("user/login", loginCommand);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginRegisterResponseModel>(responseContent);
        }

        [Fact]
        public async Task GetChatMessages_ValidRequest_ReturnsOk()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery
            {
                ChatId = 355,
                PageNumber = 1,
                PageSize = 10,
                SortBy = "SentAt",
                SearchValue = "",
                Ascending = true
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("message/get-chat-messages", query);
            response.EnsureSuccessStatusCode();
            var chatsResult = JsonConvert.DeserializeObject<IEnumerable<ChatResponseModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(chatsResult);
        }

        [Fact]
        public async Task GetUserMessages_ValidRequest_ReturnsOk()
        {
            // Arrange
            var query = new GetPagedUserMessagesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "SentAt",
                SearchValue = "",
                Ascending = true
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("message/get-user-messages", query);
            response.EnsureSuccessStatusCode();

            var messageResult = JsonConvert.DeserializeObject<IEnumerable<MessageResponseModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(messageResult);
        }

        public async Task InitializeAsync()
        {
            var authResults = await AuthenticateUserAsync();
            _accessToken = authResults.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task DisposeAsync()
        {
            await Task.WhenAll(_createdChatIds.Select(DeleteChat));
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
    }
}
