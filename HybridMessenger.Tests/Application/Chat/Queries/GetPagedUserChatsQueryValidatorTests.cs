using FluentValidation;
using FluentValidation.TestHelper;
using HybridMessenger.Application.Chat.Queries;
using System.Globalization;

namespace HybridMessenger.Tests.Application.Chat.Queries
{
    public class GetPagedUserChatsQueryValidatorTests
    {
        private readonly GetPagedUserChatsQueryValidator _validator;

        public GetPagedUserChatsQueryValidatorTests()
        {
            _validator = new GetPagedUserChatsQueryValidator();
        }

        [Fact]
        public async Task Validate_SortByInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserChatsQuery { SortBy = "InvalidPropertyName" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.SortBy)
                  .WithErrorMessage("Invalid sort property.");
        }


        [Fact]
        public async Task Validate_SortByValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserChatsQuery { SortBy = "CreatedAt" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(q => q.SortBy);
        }


        [Fact]
        public async Task Validate_FieldsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserChatsQuery { Fields = [ "InvalidPropertyName"], SortBy = "" };

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(q => q.Fields)
                  .WithErrorMessage("Invalid field(s) requested.");
        }


        [Fact]
        public async Task Validate_FieldsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetPagedUserChatsQuery { Fields = ["ChatName", "IsGroup"], SortBy = ""};
 
            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(q => q.Fields);
        }
    }
}
