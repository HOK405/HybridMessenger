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
    public class MessageControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public MessageControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/"));

            var authResults = AuthenticateUserAsync().GetAwaiter().GetResult();
            _accessToken = authResults.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
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
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(content);
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
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(content);
        }
    }
}
