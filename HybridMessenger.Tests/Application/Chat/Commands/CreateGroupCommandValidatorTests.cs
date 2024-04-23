using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Commands;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class CreateGroupCommandValidatorTests
    {
        private readonly CreateGroupCommandValidator _validator;

        public CreateGroupCommandValidatorTests()
        {
            _validator = new CreateGroupCommandValidator();
        }

        [Fact]
        public async Task Validate_ChatNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateGroupCommand { ChatName = "" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatName)
                  .WithErrorMessage("Chat name is required.");
        }

        [Fact]
        public async Task Validate_ChatNameIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateGroupCommand { ChatName = new string('a', 256) }; // 256 characters

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatName)
                  .WithErrorMessage("Chat name must be between 1 and 255 characters.");
        }

        [Fact]
        public async Task Validate_ValidChatName_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new CreateGroupCommand { ChatName = "Valid Name" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(cmd => cmd.ChatName);
        }
    }
}
