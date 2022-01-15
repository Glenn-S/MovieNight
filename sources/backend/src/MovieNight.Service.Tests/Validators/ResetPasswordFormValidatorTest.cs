using FluentAssertions;
using FluentValidation;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Service.Forms;
using MovieNight.Service.Validators;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MovieNight.Service.Tests.Validators
{
    [TestFixture]
    internal class ResetPasswordFormValidatorTest
    {
        protected IValidator<ResetPasswordForm> Sut;

        [SetUp]
        public void SetUp()
        {
            Sut = new ResetPasswordFormValidator();
        }

        [Test]
        public async Task Test_ValidatorAsync_ValidForm_Should_ReturnValidationError()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidEmail_Should_ReturnValidationError(string email)
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            testForm.Email = email;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("Email cannot be null or empty");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidToken_Should_ReturnValidationError(string token)
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            testForm.Token = token;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("The form token cannot be null or empty");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidPassword_Should_ReturnValidationError(
            string password)
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            testForm.Password = password;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            // The second error will be because the password and confirm password do not match
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("Password cannot be null or empty");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidConfirmPassword_Should_ReturnValidationError(
            string confirmPassword)
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            testForm.ConfirmPassword = confirmPassword;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("Confirm password cannot be null or empty");
        }

        [Test]
        public async Task Test_ValidatorAsync_PasswordDoesNotMatchConfirmPassword_Should_ReturnValidationError()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            testForm.Password = "Original password";
            testForm.ConfirmPassword = "Non-matching password";

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("The confirmed password does not match the password provided");
        }
    }
}
