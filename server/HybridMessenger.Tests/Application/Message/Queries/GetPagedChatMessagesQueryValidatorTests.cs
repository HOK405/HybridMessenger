using FluentValidation.TestHelper;
using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Message.Queries
{
    public class GetPagedChatMessagesQueryValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly GetPagedChatMessagesQueryValidator _validator;

        public GetPagedChatMessagesQueryValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);

            _validator = new GetPagedChatMessagesQueryValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WhenChatIdEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery { ChatId = 0, SortBy = "" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.ChatId)
                  .WithErrorMessage("Chat ID cannot be empty");
        }


        [Fact]
        public async Task Validate_WhenChatIdInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery { ChatId = 1, SortBy = "" };
            _mockChatRepository.Setup(repo => repo.ExistsAsync(query.ChatId)).ReturnsAsync(false);

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.ChatId)
                  .WithErrorMessage("Invalid chat id.");
        }


        [Fact]
        public void Validate_WhenSortByInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery { SortBy = "InvalidProperty" };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.SortBy)
                  .WithErrorMessage("Invalid sort property.");
        }


        [Fact]
        public async Task Validate_WhenFieldsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery { Fields = ["InvalidField"], SortBy = "" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.Fields)
                  .WithErrorMessage("Invalid field(s) requested.");
        }


        [Fact]
        public async Task Validate_WhenFieldsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetPagedChatMessagesQuery { Fields = ["MessageText", "SentAt"], SortBy = "" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(q => q.Fields);
        }
    }
}
