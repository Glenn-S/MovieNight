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
    public class LoginFormValidatorTest
    {
        protected IValidator<LoginForm> Sut;

        [SetUp]
        public void SetUp()
        {
            Sut = new LoginFormValidator();
        }

        [Test]
        public async Task Test_ValidatorAsync_ValidForm_Should_ReturnValidationError()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidUsername_Should_ReturnValidationError(
            string username)
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            testForm.Username = username;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("User name cannot be null or empty");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Test_ValidatorAsync_InvalidPassword_Should_ReturnValidationError(
            string password)
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            testForm.Password = password;

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().BeEquivalentTo("Password cannot be null or empty");
        }
    }
}
