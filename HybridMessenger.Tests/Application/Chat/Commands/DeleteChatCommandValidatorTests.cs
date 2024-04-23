using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Repositories;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class DeleteChatCommandValidatorTests
    {
        private readonly DeleteChatCommandValidator _validator;
        private readonly Mock<IChatRepository> _mockChatRepository;

        public DeleteChatCommandValidatorTests()
        {
            _mockChatRepository = new Mock<IChatRepository>();
            _validator = new DeleteChatCommandValidator(_mockChatRepository.Object);
        }

        [Fact]
        public async Task Validate_ChatDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            int nonExistingChatId = 1;
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(nonExistingChatId))
                .ReturnsAsync((Domain.Entities.Chat)null);

            var command = new DeleteChatCommand { ChatId = nonExistingChatId };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("The chat with this ID does not exist.");
        }

        [Fact]
        public async Task Validate_ChatExists_ShouldNotHaveValidationError()
        {
            // Arrange
            int existingChatId = 1;
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(existingChatId))
                .ReturnsAsync(new Domain.Entities.Chat()); 

            var command = new DeleteChatCommand { ChatId = existingChatId };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(cmd => cmd.ChatId);
        }
    }
}
