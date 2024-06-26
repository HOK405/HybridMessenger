﻿using FluentValidation.TestHelper;
using HybridMessenger.Application.User.Commands;

namespace HybridMessenger.Tests.Application.User.Commands
{
    public class RefreshTokenCommandValidatorTests
    {
        private readonly RefreshTokenCommandValidator _validator;

        public RefreshTokenCommandValidatorTests()
        {
            _validator = new RefreshTokenCommandValidator();
        }

        [Fact]
        public void Validate_WhenRefreshTokenEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token is required.");
        }


        [Fact]
        public void Validate_WhenRefreshTokenNull_ShouldHaveValidationError()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = null };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
                  .WithErrorMessage("Refresh token is required.");
        }


        [Fact]
        public void Validate_WhenRefreshTokenValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "valid_token" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }
    }
}
