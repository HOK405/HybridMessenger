using HybridMessenger.API;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private string _refreshToken;
        private List<int> _createdUserIds = new List<int>();

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/user/"));       
        }

        private async Task<LoginRegisterResponseModel> AuthenticateUserAsync()
        {
            var loginCommand = new VerifyByEmailPasswordCommand
            {
                Email = "testUser999@mail.com",
                Password = "testUser999"
            };

            var response = await _httpClient.PostAsJsonAsync("login", loginCommand);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginRegisterResponseModel>(responseContent);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsTokens()
        {
            // Arrange & Act
            int userId = await RegisterNewUserAsync("userToDelete1@mail.com", "userToDelete1", "userToDelete1", "+91312311231");

            // Assert
            Assert.True(userId > 0);
        }


        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange & Act
            await AuthenticateUserAsync();

            // Assert
            Assert.NotNull(_accessToken);
            Assert.NotNull(_refreshToken);
        }


        [Fact]
        public async Task GetPagedUsers_ReturnsData()
        {
            // Arrange
            var pagedQuery = new GetPagedUsersQuery
            {
                PageNumber = 1,
                PageSize = 2,
                SortBy = "CreatedAt",
                SearchValue = "",
                Ascending = true,
                Fields = { }
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("get-paged", pagedQuery);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<UserResponseModel>>(content).ToList();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count); 

            // Assert for first user details
            Assert.Equal("testUser999", result[0].UserName);
            Assert.Equal("testUser999@mail.com", result[0].Email);
            Assert.Equal("+91231212121", result[0].PhoneNumber);

            // Assert for second user details
            Assert.Equal("userToChatWith123", result[1].UserName);
            Assert.Equal("userToChatWith123@mail.com", result[1].Email);
            Assert.Equal("+12312311231", result[1].PhoneNumber);
        }


        [Fact]
        public async Task GetUser_ValidId_ReturnsOk()
        {
            // Arrange
            string expectedEmail = "testUserForGet@mail.com";
            string expectedUsername = "testUserForGet";
            string expectedPhone = "+1234567890";

            int userId = await RegisterNewUserAsync(expectedEmail, expectedUsername, "password123", expectedPhone);

            // Act
            var response = await _httpClient.GetAsync($"get-by-id?id={userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserResponseModel>(content);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(expectedEmail, result.Email);
            Assert.Equal(expectedUsername, result.UserName);
            Assert.Equal(expectedPhone, result.PhoneNumber);
        }


        [Fact]
        public async Task GetUser_InvalidId_ReturnsBadRequest()
        {
            // Arrange & Act
            var response = await _httpClient.GetAsync("get-by-id?id=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task Delete_User_ReturnsSuccess()
        {
            // Arrange
            int userIdToDelete = await RegisterNewUserAsync("userToDelete2@mail.com", "userToDelete2", "userToDelete2", "+91312311232");

            // Act
            var response = await _httpClient.PostAsJsonAsync("delete-by-id", new { UserIdToDelete = userIdToDelete });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageTextResponseModel>(content);

            // Assert
            Assert.Equal("User is successfully deleted.", result.Message);
        }

        public async Task InitializeAsync()
        {
            var authResults = await AuthenticateUserAsync();
            _accessToken = authResults.AccessToken;
            _refreshToken = authResults.RefreshToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task DisposeAsync()
        {
            await Task.WhenAll(_createdUserIds.Select(DeleteUser));
        }

        private async Task DeleteUser(int userId)
        {
            await _httpClient.PostAsJsonAsync("delete-by-id", new { UserIdToDelete = userId });
        }

        private async Task<int> RegisterNewUserAsync(string email, string userName, string password, string phoneNumber)
        {
            var newUserCommand = new RegisterUserCommand
            {
                Email = email,
                UserName = userName,
                Password = password,
                PhoneNumber = phoneNumber
            };

            var response = await _httpClient.PostAsJsonAsync("register", newUserCommand);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(content);

            var userId = GetUserIdFromJwt(result.AccessToken);
            _createdUserIds.Add(userId);
            return userId;
        }


        private int GetUserIdFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var idClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;

            if (int.TryParse(idClaim, out int userId))
            {
                return userId;
            }
            throw new InvalidOperationException("Invalid user ID claim in the JWT.");
        }
    }
}
