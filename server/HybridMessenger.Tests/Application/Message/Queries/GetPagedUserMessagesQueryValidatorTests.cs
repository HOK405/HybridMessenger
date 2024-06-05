using FluentValidation.TestHelper;
using HybridMessenger.Application.Message.Queries;

namespace HybridMessenger.Tests.Application.Message.Queries
{
    public class GetPagedUserMessagesQueryValidatorTests
    {
        private readonly GetPagedUserMessagesQueryValidator _validator;

        public GetPagedUserMessagesQueryValidatorTests()
        {
            _validator = new GetPagedUserMessagesQueryValidator();
        }

        [Fact]
        public void SortBy_WhenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserMessagesQuery { SortBy = "MessageText" }; 

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(q => q.SortBy);
        }


        [Fact]
        public void SortBy_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserMessagesQuery { SortBy = "InvalidProperty" };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.SortBy)
                  .WithErrorMessage("Invalid sort property.");
        }


        [Fact]
        public void Fields_WhenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserMessagesQuery { Fields = ["MessageText", "SentAt"], SortBy = "" }; 

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(q => q.Fields);
        }


        [Fact]
        public void Fields_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserMessagesQuery { Fields = ["InvalidField"], SortBy = "" };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.Fields)
                  .WithErrorMessage("Invalid field(s) requested.");
        }
    }
}
