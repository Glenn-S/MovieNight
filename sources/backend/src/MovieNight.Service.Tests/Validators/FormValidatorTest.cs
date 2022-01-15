using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MovieNight.Service.Attributes;
using MovieNight.Service.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieNight.Service.Tests.Validators
{
    [TestFixture]
    internal class FormValidatorTest
    {
        protected IFormValidator Sut;

        private MockRepository _mockRepository;
        private Mock<TestValidValidator> _mockTestValidator;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _mockTestValidator = _mockRepository.Create<TestValidValidator>();

            Sut = new FormValidator(new[] { _mockTestValidator.Object });
        }

        #region Constructor Tests

        [Test]
        public void Test_Ctor_NullValidatorList_Should_ThrowArgumentNullException()
        {
            // Act
            Func<FormValidator> result = () => new FormValidator(null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'validators')");
        }

        [Test]
        public void Test_Ctor_InValidValidatorInList_Should_ThrowArgumentException()
        {
            // Assign
            var validatorList = new List<IValidator>() { new TestInValidValidator() };

            // Act
            Func<FormValidator> result = () => new FormValidator(validatorList);

            // Assert
            result.Should().ThrowExactly<ArgumentException>()
                .WithMessage("The validator 'TestInValidValidator' did not have a validator type associated with it");
        }

        [Test]
        public void Test_Ctor_ValidValidatorsList_Should_NotThrowArgumentException()
        {
            // Assign
            var validatorList = new List<IValidator>() { new TestValidValidator() };

            // Act
            Func<FormValidator> result = () => new FormValidator(validatorList);

            // Assert
            result.Should().NotThrow<ArgumentException>();
        }

        #endregion

        #region Validate Tests

        [Test]
        public void Test_Validate_ValidType_Should_Validate()
        {
            // Assign
            var testForm = "Some string to pretend is a form";

            _mockTestValidator
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(new ValidationResult());

            // Act
            var result = Sut.Validate(testForm);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Test_Validate_ValidatorTypeDoesNotMatchTypePassedIn_Should_ThrowKeyNotFoundException()
        {
            // Assign
            var testForm = new object();

            // Act
            Func<ValidationResult> result = () => Sut.Validate(testForm);

            // Assert
            result.Should().ThrowExactly<KeyNotFoundException>()
                .WithMessage("A validator for the form type 'Object' does not exist");
        }

        #endregion

        #region ValidateAsync Tests

        [Test]
        public async Task Test_ValidateAsync_ValidType_Should_Validate()
        {
            // Assign
            var testForm = "Some string to pretend is a form";

            _mockTestValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IValidationContext>(), default))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await Sut.ValidateAsync(testForm);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Test_ValidateAsync_ValidatorTypeDoesNotMatchTypePassedIn_Should_ThrowKeyNotFoundException()
        {
            // Assign
            var testForm = new object();

            // Act
            Func<ValidationResult> result = () => Sut.Validate(testForm);

            // Assert
            result.Should().ThrowExactly<KeyNotFoundException>()
                .WithMessage("A validator for the form type 'Object' does not exist");
        }

        #endregion

        #region TestHelpers

        // Class with no ValidatorTypeAttribute
        internal class TestInValidValidator :
            IValidator
        {
            public bool CanValidateInstancesOfType(Type type) => throw new NotImplementedException();
            public IValidatorDescriptor CreateDescriptor() => throw new NotImplementedException();
            public virtual ValidationResult Validate(IValidationContext context) => throw new NotImplementedException();
            public virtual Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default) => throw new NotImplementedException();
        }

        [ValidatorType(typeof(string))]
        internal class TestValidValidator :
            TestInValidValidator
        { }

        #endregion
    }
}
