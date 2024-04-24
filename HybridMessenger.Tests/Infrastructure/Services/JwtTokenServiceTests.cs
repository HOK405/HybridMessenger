using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Services;
using HybridMessenger.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HybridMessenger.Tests.Infrastructure.Services
{
    public class JwtTokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUserIdentityService> _mockIdentityService;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockIdentityService = new Mock<IUserIdentityService>();
            _jwtTokenService = new JwtTokenService(_mockConfiguration.Object, _mockIdentityService.Object);

            _mockConfiguration.SetupGet(x => x["JwtSettings:Key"]).Returns("very_secret_key_that_is_long_enough");
            _mockConfiguration.SetupGet(x => x["JwtSettings:Issuer"]).Returns("ExampleIssuer");
            _mockConfiguration.SetupGet(x => x["JwtSettings:Audience"]).Returns("ExampleAudience");
        }

        [Fact]
        public async Task GenerateAccessToken_ReturnsValidTokenWithCorrectClaims()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "JohnDoe", Email = "john@example.com" };
            var roles = new List<string> { "Admin", "User" };
            _mockIdentityService.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var token = await _jwtTokenService.GenerateAccessToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == "JohnDoe");
            Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == "john@example.com");
            Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "Admin");
            Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "User");
        }


        [Fact]
        public async Task GenerateRefreshToken_ReturnsValidToken()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "JohnDoe", Email = "john@example.com" };

            // Act
            var token = await _jwtTokenService.GenerateRefreshToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.NotNull(jwtToken);
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }


        [Fact]
        public async Task GetPrincipalFromExpiredToken_ReturnsValidPrincipal()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "JohnDoe", Email = "john@example.com" };
            var roles = new List<string>(); 

            _mockIdentityService.Setup(x => x.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

            var token = await _jwtTokenService.GenerateAccessToken(user); 

            // Act
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);

            // Assert
            Assert.NotNull(principal);
            var nameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            Assert.NotNull(nameClaim);
            Assert.Equal("JohnDoe", nameClaim.Value);
        }
    }
}
