using HybridMessenger.API;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private string _refreshToken;
        private List<int> _createdUserIds = new List<int>();

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/user/"));

            var authResults = AuthenticateUserAsync().GetAwaiter().GetResult();
            _accessToken = authResults.AccessToken;
            _refreshToken = authResults.RefreshToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
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

        public void Dispose()
        {
            foreach (var userId in _createdUserIds)
            {
                DeleteUser(userId).Wait();
            }
        }

        private async Task DeleteUser(int userId)
        {
            await _httpClient.PostAsJsonAsync("delete-by-id", new { UserIdToDelete = userId });
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
            // Ensure user is authenticated
            await AuthenticateUserAsync();

            // Create the paged query
            var pagedQuery = new GetPagedUsersQuery
            {
                PageNumber = 1,
                PageSize = 1,
                SortBy = "CreatedAt",
                SearchValue = "",
                Ascending = true,
                Fields = { }
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("get-paged", pagedQuery);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<UserResponseModel>>(content);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetUser_ValidId_ReturnsOk()
        {
            // Arrange
            int userId = await RegisterNewUserAsync("testUserForGet@mail.com", "testUserForGet", "password123", "+1234567890");

            // Act
            var response = await _httpClient.GetAsync($"get-by-id?id={userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserResponseModel>(content);

            // Assert
            Assert.NotNull(result);
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
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            Assert.Equal("User is successfully deleted.", result.Message);
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
