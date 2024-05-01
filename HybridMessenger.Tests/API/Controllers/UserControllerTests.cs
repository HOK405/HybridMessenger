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
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private string _refreshToken;
        private int _loggedUserId;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/user/"));
        }

        private async Task AuthenticateUser()
        {
            var command = new VerifyByEmailPasswordCommand
            {
                Email = "testUser999@mail.com",
                Password = "testUser999"
            };

            var response = await _httpClient.PostAsJsonAsync("login", command);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(responseContent);

            _accessToken = result.AccessToken;
            _refreshToken = result.RefreshToken;
            _loggedUserId = GetUserIdFromJwt(_accessToken); 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsTokens()
        {
            // Arrange
            var newUserCommand = new RegisterUserCommand
            {
                Email = "userToDelete1@mail.com",
                UserName = "userToDelete1",
                Password = "userToDelete1",
                PhoneNumber = "+91312311231"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("register", newUserCommand);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(content);

            // Assert
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange & Act
            await AuthenticateUser();

            // Assert
            Assert.NotNull(_accessToken);
            Assert.NotNull(_refreshToken);
        }

        [Fact]
        public async Task GetPagedUsers_ReturnsData()
        {
            // Ensure user is authenticated
            await AuthenticateUser();

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
            // Ensure user is authenticated
            await AuthenticateUser();

            // Act
            var response = await _httpClient.GetAsync($"get-by-id?id={_loggedUserId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserResponseModel>(content);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Delete_User_ReturnsSuccess()
        {
            // Ensure user is authenticated
            await AuthenticateUser();

            int userIdToDelete = await GetNewlyRegisteredUser();

            // Act
            var response = await _httpClient.PostAsJsonAsync("delete-by-id", new { UserIdToDelete = userIdToDelete });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MessageResponseModel>(content);

            // Assert
            Assert.Equal("User is successfully deleted.", result.Message);
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

        private async Task<int> GetNewlyRegisteredUser()
        {
            // Create the paged query
            var pagedQuery = new GetPagedUsersQuery
            {
                PageNumber = 1,
                PageSize = 1,
                SortBy = "CreatedAt",
                SearchValue = "userToDelete1",
                Ascending = true,
                Fields = { }
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("get-paged", pagedQuery);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<UserResponseModel>>(content);

            if (result != null && result.Any())
            {
                return result.First().Id; 
            }
            else
            {
                throw new InvalidOperationException("No users found.");
            }
        }
    }
}
