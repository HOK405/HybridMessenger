using HybridMessenger.API;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Tests.API.ResponseModels;
using HybridMessenger.Tests.API.Settings;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace HybridMessenger.Tests.API.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient(new Uri("https://localhost/api/user/"));
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            VerifyByEmailPasswordCommand command = new VerifyByEmailPasswordCommand()
            {
                Email = "firstUserTest123@mail.com",
                Password = "firstUserTest123"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("login", command);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task GetUser_ValidId_ReturnsOk()
        {
            // Arrange: Login to obtain the access token
            var command = new VerifyByEmailPasswordCommand
            {
                Email = "firstUserTest123@mail.com",
                Password = "firstUserTest123"
            };

            // Act: Send login request
            var loginResponse = await _httpClient.PostAsJsonAsync("login", command);
            loginResponse.EnsureSuccessStatusCode(); 
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(loginResponseContent);

            // Set the authorization header with the received access token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

            // Act: Request user data
            int userId = 2;
            var userResponse = await _httpClient.GetAsync($"get-by-id?id={userId}");
            userResponse.EnsureSuccessStatusCode();
            var userResponseContent = await userResponse.Content.ReadAsStringAsync();
            var userResponseModelresult = JsonConvert.DeserializeObject<UserResponseModel>(userResponseContent);

            // Assert
            Assert.NotNull(userResponseModelresult);
        }

        [Fact]
        public async Task GetPagedUsers_ReturnsData()
        {
            // Arrange: Login to obtain the access token
            var command = new VerifyByEmailPasswordCommand
            {
                Email = "firstUserTest123@mail.com",
                Password = "firstUserTest123"
            };
            var loginResponse = await _httpClient.PostAsJsonAsync("login", command);
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(await loginResponse.Content.ReadAsStringAsync());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

            // Create the paged query
            var pagedQuery = new GetPagedUsersQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "CreatedAt",
                SearchValue = "test",
                Ascending = true
            };

            // Act
            var pagedResponse = await _httpClient.PostAsJsonAsync("get-paged", pagedQuery);
            pagedResponse.EnsureSuccessStatusCode();
            var pagedContent = await pagedResponse.Content.ReadAsStringAsync();
            var pagedResult = JsonConvert.DeserializeObject<IEnumerable<object>>(pagedContent); 

            // Assert
            Assert.NotNull(pagedResult);
            Assert.NotEmpty(pagedResult);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsTokens()
        {
            // Arrange
            string uniquePart = Guid.NewGuid().ToString("N"); // "N" format removes the dashes
            string username = $"user{uniquePart}";
            string email = $"test{uniquePart}@example.com";
            string password = $"password{uniquePart}"; 

            var newUserCommand = new RegisterUserCommand
            {
                UserName = username,
                Email = email,
                Password = password
            };

            // Act
            var registerResponse = await _httpClient.PostAsJsonAsync("register", newUserCommand);
            registerResponse.EnsureSuccessStatusCode(); 
            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            var registerResult = JsonConvert.DeserializeObject<LoginRegisterResponseModel>(registerContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
            Assert.NotNull(registerResult.AccessToken);
            Assert.NotNull(registerResult.RefreshToken);
        }
    }
}
