using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class CreatePrivateChatCommandValidatorTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly CreatePrivateChatCommandValidator _validator;

        public CreatePrivateChatCommandValidatorTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new CreatePrivateChatCommandValidator(_mockUserRepository.Object);
        }

        [Fact]
        public async Task Validate_UsernameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = "" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.UserNameToCreateWith)
                  .WithErrorMessage("Username to create chat with cannot be empty.");
        }

        [Fact]
        public async Task Validate_UsernameDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = "nonexistentUser" };
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync("nonexistentUser")).ReturnsAsync((Domain.Entities.User)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.UserNameToCreateWith)
                  .WithErrorMessage("The user with this username does not exist.");
        }

        [Fact]
        public async Task Validate_ValidUsername_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new CreatePrivateChatCommand { UserNameToCreateWith = "existingUser" };
            var user = new Domain.Entities.User { UserName = "existingUser" };
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync("existingUser")).ReturnsAsync(user);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(cmd => cmd.UserNameToCreateWith);
        }
    }

}
