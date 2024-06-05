using FluentValidation.TestHelper;
using HybridMessenger.Application.User.Commands;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator;

        public RegisterUserCommandValidatorTests()
        {
            _validator = new RegisterUserCommandValidator();
        }


        [Fact]
        public void Validate_WhenUsernameEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { UserName = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("Username is required.");
        }

        [Fact]
        public void Validate_WhenUsernameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { UserName = new string('a', 256) }; // 256 characters

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("Username must be between 1 and 255 characters.");
        }


        [Fact]
        public void Validate_WhenPasswordEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { Password = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void Validate_WhenPasswordTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { Password = "12345" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 6 characters long.");
        }


        [Fact]
        public void Validate_WhenEmailEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void Validate_WhenEmailInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { Email = "invalid-email" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("A valid email is required.");
        }


        [Fact]
        public void Validate_WhenPhoneNumberInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { PhoneNumber = "invalidPhone" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                  .WithErrorMessage("A valid phone number is required.");
        }

        [Fact]
        public void Validate_WhenPhoneNumberValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new RegisterUserCommand { PhoneNumber = "+123456789012" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        }
    }
}
