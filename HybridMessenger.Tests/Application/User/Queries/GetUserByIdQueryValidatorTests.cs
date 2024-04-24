using FluentValidation.TestHelper;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Domain.Services;
using Moq;

namespace HybridMessenger.Tests.Application.User.Queries
{
    public class GetUserByIdQueryValidatorTests
    {
        private readonly Mock<IUserIdentityService> _mockUserIdentityService;
        private readonly GetUserByIdQueryValidator _validator;

        public GetUserByIdQueryValidatorTests()
        {
            _mockUserIdentityService = new Mock<IUserIdentityService>();
            _validator = new GetUserByIdQueryValidator(_mockUserIdentityService.Object);
        }

        [Fact]
        public void Id_WhenEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 0 }; // Assuming ID is an integer and 0 is treated as empty

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("User ID is required.");
        }


        [Fact]
        public async Task Id_WhenUserDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };
            _mockUserIdentityService.Setup(x => x.GetUserByIdAsync(query.Id)).ReturnsAsync((Domain.Entities.User)null);

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("No user found with the given ID.");
        }


        [Fact]
        public async Task Id_WhenUserExists_ShouldNotHaveValidationError()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };
            var user = new Domain.Entities.User { Id = 1, UserName = "ExistingUser" };
            _mockUserIdentityService.Setup(x => x.GetUserByIdAsync(query.Id)).ReturnsAsync(user);

            // Act
            var result = await _validator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
