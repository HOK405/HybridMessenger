using FluentValidation.TestHelper;
using HybridMessenger.Application.User.Queries;

namespace HybridMessenger.Tests.Application.User.Queries
{
    public class GetPagedUsersQueryValidatorTests
    {
        private readonly GetPagedUsersQueryValidator _validator;

        public GetPagedUsersQueryValidatorTests()
        {
            _validator = new GetPagedUsersQueryValidator();
        }


        [Fact]
        public void SortBy_WhenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new GetPagedUsersQuery { SortBy = "UserName" }; 

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
        }


        [Fact]
        public void SortBy_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new GetPagedUsersQuery { SortBy = "InvalidProperty" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SortBy)
                  .WithErrorMessage("Invalid sort property.");
        }


        [Fact]
        public void Fields_WhenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new GetPagedUsersQuery { Fields = ["UserName", "Email"], SortBy = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Fields);
        }


        [Fact]
        public void Fields_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new GetPagedUsersQuery { Fields = ["InvalidField"], SortBy = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Fields)
                  .WithErrorMessage("Invalid field(s) requested.");
        }
    }
}
