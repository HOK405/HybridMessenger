using HybridMessenger.API;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class MessageControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public MessageControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/"));

            var authResults = AuthenticateUserAsync().GetAwaiter().GetResult();
            _accessToken = authResults.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<LoginRegisterResponseModel> AuthenticateUserAsync()
        {
            var loginCommand = DefaultUserData.GetLoginCommand();
            return await _httpClient.PostAsJsonAsync<LoginRegisterResponseModel>("user/login", loginCommand);
        }


        [Fact]
        public async Task GetChatMessages_ValidRequest_ReturnsOk()
        {
            // Arrange
            var expectedMessages = DefaultMessageData.GetMessages().Where(m => m.ChatId == 355).ToList();
            var query = DefaultMessageData.GetPagedChatMessagesQuery();

            // Act
            var messagesResult = await _httpClient.PostAsJsonAsync<List<MessageResponseModel>>("message/get-chat-messages", query);

            // Assert
            Assert.NotEmpty(messagesResult);
            Assert.Equal(expectedMessages.Count, messagesResult.Count);
            Assert.Equal(expectedMessages, messagesResult, new MessageResponseModelComparer());
        }


        [Fact]
        public async Task GetUserMessages_ValidRequest_ReturnsOk()
        {
            // Arrange
            var expectedMessages = DefaultMessageData.GetMessages().Where(m => m.UserId == 31).ToList();
            var query = DefaultMessageData.GetPagedUserMessagesQuery();

            // Act
            var messagesResult = await _httpClient.PostAsJsonAsync<List<MessageResponseModel>>("message/get-user-messages", query);

            // Assert
            Assert.NotNull(messagesResult);
            Assert.Equal(expectedMessages.Count, messagesResult.Count);
            Assert.Equal(expectedMessages, messagesResult, new MessageResponseModelComparer());
        }

        private class MessageResponseModelComparer : IEqualityComparer<MessageResponseModel>
        {
            public bool Equals(MessageResponseModel x, MessageResponseModel y)
            {
                return x.MessageId == y.MessageId && x.ChatId == y.ChatId && x.UserId == y.UserId &&
                       x.MessageText == y.MessageText && x.SenderUserName == y.SenderUserName;
            }

            public int GetHashCode(MessageResponseModel obj)
            {
                return obj.MessageId.GetHashCode();
            }
        }
    }
}