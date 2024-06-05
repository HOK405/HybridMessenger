using HybridMessenger.Infrastructure.Services;
using System.Security.Claims;

namespace HybridMessenger.Tests.Infrastructure.Services
{
    public class UserClaimsServiceTests
    {
        private readonly UserClaimsService _service;

        public UserClaimsServiceTests()
        {
            _service = new UserClaimsService();
        }


        [Fact]
        public void GetUserId_ValidClaim_ReturnsUserId()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "123")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var userId = _service.GetUserId(principal);

            // Assert
            Assert.Equal(123, userId);
        }


        [Fact]
        public void GetUserId_InvalidClaim_ThrowsInvalidOperationException()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.GetUserId(principal));
        }


        [Fact]
        public void GetUserId_ClaimMissing_ThrowsInvalidOperationException()
        {
            // Arrange
            var claims = new List<Claim>(); 
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.GetUserId(principal));
        }
    }
}
