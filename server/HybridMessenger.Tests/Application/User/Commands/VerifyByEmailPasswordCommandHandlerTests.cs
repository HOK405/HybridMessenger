using HybridMessenger.Application.User.Commands;
using HybridMessenger.Domain.Services;
using Moq;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class VerifyByEmailPasswordCommandHandlerTests
    {
        private readonly Mock<IUserIdentityService> _mockUserService;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly VerifyByEmailPasswordCommandHandler _handler;

        public VerifyByEmailPasswordCommandHandlerTests()
        {
            _mockUserService = new Mock<IUserIdentityService>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _handler = new VerifyByEmailPasswordCommandHandler(_mockUserService.Object, _mockJwtTokenService.Object);
        }


        [Fact]
        public async Task Handle_ValidCredentials_ReturnsTokens()
        {
            // Arrange
            var request = new VerifyByEmailPasswordCommand { Email = "user@example.com", Password = "securePassword123" };
            var user = new Domain.Entities.User { Id = 1, UserName = "user@example.com" };
            var accessToken = "access_token";
            var refreshToken = "refresh_token";

            _mockUserService.Setup(x => x.VerifyUserByEmailAndPasswordAsync(request.Email, request.Password))
                            .ReturnsAsync(user);
            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user))
                                .ReturnsAsync(accessToken);
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken(user))
                                .ReturnsAsync(refreshToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(accessToken, result.Item1);
            Assert.Equal(refreshToken, result.Item2);
            _mockUserService.Verify(x => x.VerifyUserByEmailAndPasswordAsync(request.Email, request.Password), Times.Once);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(user), Times.Once);
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(user), Times.Once);
        }
    }
}
