using FluentValidation;
using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class ChangeGroupNameCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly ChangeGroupNameCommandValidator _validator;

        public ChangeGroupNameCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);

            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

            _validator = new ChangeGroupNameCommandValidator(_mockUnitOfWork.Object);
        }


        [Fact]
        public async Task Validate_ChatIdIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { ChatId = 0, NewChatName = "Valid Name", UserId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Chat ID cannot be empty");
        }

        [Fact]
        public async Task Validate_ChatIdDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { ChatId = 999, NewChatName = "Valid Name", UserId = 1 };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Invalid chat id.");
        }

        [Fact]
        public async Task Validate_ChatIsNotGroupChat_ShouldHaveValidationError()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { ChatId = 1, NewChatName = "Valid Name", UserId = 1 };
            var chat = new Domain.Entities.Chat { ChatId = 1, IsGroup = false };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(chat);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Only group chats can have their names changed.");
        }

        [Fact]
        public async Task Validate_NewChatNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { ChatId = 1, NewChatName = "", UserId = 1 };
            var chat = new Domain.Entities.Chat { ChatId = 1, IsGroup = true };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(chat);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.NewChatName)
                  .WithErrorMessage("New chat name cannot be empty");
        }

        [Fact]
        public async Task Validate_NewChatNameIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { ChatId = 1, NewChatName = new string('a', 256), UserId = 1 }; // 256 characters
            var chat = new Domain.Entities.Chat { ChatId = 1, IsGroup = true };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(chat);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.NewChatName)
                  .WithErrorMessage("New chat name must be between 1 and 255 characters");
        }
    }

}
