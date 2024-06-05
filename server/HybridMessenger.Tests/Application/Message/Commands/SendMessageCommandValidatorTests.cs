using FluentValidation.TestHelper;
using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Message.Commands
{
    public class SendMessageCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly SendMessageCommandValidator _validator;

        public SendMessageCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);

            _validator = new SendMessageCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task ChatId_WhenEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = 0, MessageText = "Hello, World!" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Chat ID cannot be empty");
        }

        [Fact]
        public async Task ChatId_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = 1, MessageText = "Hello, World!" };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(command.ChatId)).ReturnsAsync(false);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Invalid chat id.");
        }


        [Fact]
        public async Task MessageText_WhenEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = 1, MessageText = "" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.MessageText)
                  .WithErrorMessage("Message cannot be empty");
        }


        [Fact]
        public async Task ChatId_WhenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new SendMessageCommand { ChatId = 1, MessageText = "Hello, World!" };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(command.ChatId)).ReturnsAsync(true);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(cmd => cmd.ChatId);
        }
    }
}
