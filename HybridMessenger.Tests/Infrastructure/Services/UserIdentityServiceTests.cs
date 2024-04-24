using HybridMessenger.Domain.Entities;
using HybridMessenger.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HybridMessenger.Tests.Infrastructure.Services
{
    public class UserIdentityServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly UserIdentityService _userIdentityService;

        public UserIdentityServiceTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _userIdentityService = new UserIdentityService(_mockUserManager.Object);
        }


        [Fact]
        public async Task CreateUserAsync_Success_ReturnsIdentityResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com", UserName = "testuser" };
            var password = "Password123!";
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userIdentityService.CreateUserAsync(user, password);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task CreateUserAsync_Failure_ThrowsArgumentException()
        {
            // Arrange
            var user = new User { Email = "test@example.com", UserName = "testuser" };
            var password = "Password123!";
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid password" }));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userIdentityService.CreateUserAsync(user, password));
            Assert.Contains("Invalid password", exception.Message);
        }


        [Fact]
        public async Task VerifyUserByEmailAndPasswordAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "test@example.com", UserName = "testuser" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userIdentityService.VerifyUserByEmailAndPasswordAsync("test@example.com", "Password123!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
        }

        [Fact]
        public async Task VerifyUserByEmailAndPasswordAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userIdentityService.VerifyUserByEmailAndPasswordAsync("test@example.com", "Password123!");

            // Assert
            Assert.Null(result);
        }
    }
}
