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
    internal class VerifyEmailFormValidatorTest
    {
        protected IValidator<VerifyEmailForm> Sut;

        [SetUp]
        public void SetUp()
        {
            Sut = new VerifyEmailFormValidator();
        }

        [Test]
        public async Task Test_ValidatorAsync_ValidForm_Should_ReturnValidationError()
        {
            // Assign
            var testForm = new VerifyEmailFormFaker().Generate();

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
            var testForm = new VerifyEmailFormFaker().Generate();
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
            var testForm = new VerifyEmailFormFaker().Generate();
            testForm.Token = token;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("The form token cannot be null or empty");
        }
    }
}
