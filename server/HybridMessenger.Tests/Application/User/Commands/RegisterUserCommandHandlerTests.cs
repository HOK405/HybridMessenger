using HybridMessenger.Application.User.Commands;
using HybridMessenger.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserIdentityService> _mockUserService;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _mockUserService = new Mock<IUserIdentityService>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _handler = new RegisterUserCommandHandler(_mockUserService.Object, _mockJwtTokenService.Object);
        }

        [Fact]
        public async Task Handle_Success_ReturnsTokens()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                PhoneNumber = "1234567890"
            };
            var user = new Domain.Entities.User
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };
            var identityResultSuccess = IdentityResult.Success; // Assuming IdentityResult has a static success method or property
            var accessToken = "access_token";
            var refreshToken = "refresh_token";

            _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<Domain.Entities.User>(), request.Password))
                            .ReturnsAsync(identityResultSuccess);
            _mockUserService.Setup(x => x.AddRoleAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<string>()))
                            .ReturnsAsync(identityResultSuccess);
            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<Domain.Entities.User>()))
                                .ReturnsAsync(accessToken);
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<Domain.Entities.User>()))
                                .ReturnsAsync(refreshToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(accessToken, result.Item1);
            Assert.Equal(refreshToken, result.Item2);
            _mockUserService.Verify(x => x.CreateUserAsync(It.IsAny<Domain.Entities.User>(), request.Password), Times.Once);
            _mockUserService.Verify(x => x.AddRoleAsync(It.IsAny<Domain.Entities.User>(), "Default"), Times.Once);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<Domain.Entities.User>()), Times.Once);
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<Domain.Entities.User>()), Times.Once);
        }
    }
}
