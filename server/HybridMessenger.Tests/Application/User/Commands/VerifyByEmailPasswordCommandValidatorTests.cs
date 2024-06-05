using FluentValidation.TestHelper;
using HybridMessenger.Application.User.Commands;
using HybridMessenger.Domain.Services;
using Moq;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class VerifyByEmailPasswordCommandValidatorTests
    {
        private readonly Mock<IUserIdentityService> _mockUserService;
        private readonly VerifyByEmailPasswordCommandValidator _validator;

        public VerifyByEmailPasswordCommandValidatorTests()
        {
            _mockUserService = new Mock<IUserIdentityService>();
            _validator = new VerifyByEmailPasswordCommandValidator(_mockUserService.Object);
        }


        [Fact]
        public void Email_WhenEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new VerifyByEmailPasswordCommand { Email = "", Password = "ValidPass123!" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }


        [Fact]
        public void Email_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new VerifyByEmailPasswordCommand { Email = "invalid-email", Password = "ValidPass123!" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("A valid email is required.");
        }


        [Fact]
        public void Password_WhenTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var command = new VerifyByEmailPasswordCommand { Email = "valid@example.com", Password = "short" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 6 characters long.");
        }


        [Fact]
        public async Task Credentials_WhenInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new VerifyByEmailPasswordCommand { Email = "valid123@example.com", Password = "ValidPass123!" };
            _mockUserService.Setup(x => x.VerifyUserByEmailAndPasswordAsync(command.Email, command.Password))
                            .ReturnsAsync((Domain.Entities.User)null);  // Simulating invalid credentials

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveAnyValidationError()
                  .WithErrorMessage("Invalid email or password.");
        }

    }
}
