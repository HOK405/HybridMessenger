using HybridMessenger.Application.User.Commands;
using HybridMessenger.Domain.Services;
using Moq;
using System.Security.Claims;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly Mock<IUserIdentityService> _mockUserService;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _mockUserService = new Mock<IUserIdentityService>();
            _handler = new RefreshTokenCommandHandler(_mockJwtTokenService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task Handle_InvalidToken_ThrowsArgumentException()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "invalid_token" };
            _mockJwtTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns((ClaimsPrincipal)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_MissingUserIdInToken_ThrowsArgumentException()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "some_token" };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }));
            _mockJwtTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "some_token" };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));
            _mockJwtTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            _mockUserService.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ValidToken_ReturnsNewAccessToken()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "valid_token" };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));
            var user = new Domain.Entities.User { Id = 1 };
            var newAccessToken = "new_access_token";

            _mockJwtTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            _mockUserService.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user)).ReturnsAsync(newAccessToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(newAccessToken, result);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(user), Times.Once);
        }
    }
}
