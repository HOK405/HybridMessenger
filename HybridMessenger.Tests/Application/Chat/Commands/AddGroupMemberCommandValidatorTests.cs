using FluentValidation;
using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class AddGroupMemberCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IChatMemberRepository> _mockChatMemberRepository;
        private readonly AddGroupMemberCommandValidator _validator;

        public AddGroupMemberCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockChatMemberRepository = new Mock<IChatMemberRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatMemberRepository>()).Returns(_mockChatMemberRepository.Object);

            _validator = new AddGroupMemberCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_CommandWithEmptyUsername_ShouldHaveValidationError()
        {
            // Arrange
            var command = new AddGroupMemberCommand { UserNameToAdd = "", ChatId = 1 };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.UserNameToAdd)
                  .WithErrorMessage("Username cannot be empty.");
        }


        [Fact]
        public async Task Validate_InvalidUsername_ShouldHaveValidationError()
        {
            // Arrange
            var command = new AddGroupMemberCommand { UserNameToAdd = "nonexistent", ChatId = 1 };
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync("nonexistent"))
                               .ReturnsAsync((Domain.Entities.User)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.UserNameToAdd)
                  .WithErrorMessage("The provided username is not valid.");
        }


        [Fact]
        public async Task Validate_UserAlreadyInChat_ShouldHaveValidationError()
        {
            // Arrange
            var command = new AddGroupMemberCommand { UserNameToAdd = "existingUser", ChatId = 1 };
            var user = new Domain.Entities.User { Id = 2, UserName = "existingUser", CreatedAt = DateTime.UtcNow };
            var chat = new Domain.Entities.Chat { ChatId = 1, IsGroup = true };

            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync("existingUser"))
                               .ReturnsAsync(user);
            _mockChatMemberRepository.Setup(repo => repo.IsUserMemberOfChatAsync(user.Id, command.ChatId))
                                     .ReturnsAsync(true);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(command.ChatId))
                               .ReturnsAsync(chat);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.UserNameToAdd)
                  .WithErrorMessage("The provided user is already in this chat.");
        }


        [Fact]
        public async Task Validate_EmptyChatId_ShouldHaveValidationError()
        {
            // Arrange
            var command = new AddGroupMemberCommand { ChatId = 0, UserNameToAdd = "existingUser" };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("Chat ID cannot be empty.");
        }


        [Fact]
        public async Task Validate_ChatIsNotGroupChat_ShouldHaveValidationError()
        {
            // Arrange
            var command = new AddGroupMemberCommand { ChatId = 1, UserNameToAdd = "existingUser" };
            var chat = new Domain.Entities.Chat { ChatId = 1, IsGroup = false };
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(command.ChatId))
                               .ReturnsAsync(chat);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(cmd => cmd.ChatId)
                  .WithErrorMessage("New users can't be added to private chats.");
        }
    }
}
