using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MovieNight.Common.Entities;
using MovieNight.Common.Results;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Common.Testing.Mocks;
using MovieNight.Service.Services;
using MovieNight.Service.Validators;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MovieNight.Service.Tests.Services
{
    [TestFixture]
    public class AccountServiceTest
    {
        protected IAccountService Sut;

        private MockRepository _mockRepository;
        private ILogger<AccountService> _mockLogger;
        private MockUserManager<UserEntity> _mockUserManager;
        private MockSignInManager<UserEntity> _mockSignInManager;
        private Mock<IFormValidator> _mockFormValidator;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);

            _mockLogger = new NullLogger<AccountService>();
            _mockUserManager = new MockUserManager<UserEntity>();
            _mockSignInManager = new MockSignInManager<UserEntity>();
            _mockFormValidator = _mockRepository.Create<IFormValidator>();

            Sut = new AccountService(
                _mockLogger,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockFormValidator.Object);
        }

        #region Constructor Tests

        [Test]
        public void Test_Ctor_NullLogger_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AccountService> result = () => new AccountService(
                null,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockFormValidator.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'logger')");
        }

        [Test]
        public void Test_Ctor_NullUserManager_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AccountService> result = () => new AccountService(
                _mockLogger,
                null,
                _mockSignInManager.Object,
                _mockFormValidator.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'userManager')");
        }

        [Test]
        public void Test_Ctor_NullSignInManager_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AccountService> result = () => new AccountService(
                _mockLogger,
                _mockUserManager.Object,
                null,
                _mockFormValidator.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'signInManager')");
        }

        [Test]
        public void Test_Ctor_NullFormValidator_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AccountService> result = () => new AccountService(
                _mockLogger,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'registrationFormValidator')");
        }

        #endregion

        #region RegisterUser Tests

        [Test]
        public async Task Test_RegisterUser_ValidForm_Should_ReturnOk()
        {
            // Assign
            var testForm = new RegistrationFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), testForm.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await Sut.RegisterUser(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status201Created);
            var actualResult = ((CreatedAtRouteResult)result).Value as ResultModel<string>;
            actualResult.Should().NotBeNull();
            actualResult.Data.Should().BeEquivalentTo($"User with email '{testForm.Email}' has been created.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_RegisterUser_NullForm_Should_ReturnBadRequest()
        {
            // Act
            var result = await Sut.RegisterUser(null);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("The registration form provided was null.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_RegisterUser_ValidationFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new RegistrationFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("password", "Password cannot be null or empty", testForm.Password)
                }));

            // Act
            var result = await Sut.RegisterUser(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code:  Attempted Value: {testForm.Password} Message: Password cannot be null or empty");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_RegisterUser_CreateUserFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new RegistrationFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), testForm.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordLength",
                    Description = "The password was not long enough"
                }));

            // Act
            var result = await Sut.RegisterUser(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code: PasswordLength Message: The password was not long enough");
            _mockRepository.Verify();
        }

        #endregion

        #region LoginUser Tests

        [Test]
        public async Task Test_Login_ValidForm_Should_ReturnOk()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status200OK);
            var actualResult = ((OkObjectResult)result).Value;
            result.Should().NotBeNull();
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_NullForm_Should_ReturnBadRequest()
        {
            // Act
            var result = await Sut.Login(null);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("The login form provided was null.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_ValidataionFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("password", "Password cannot be null or empty", testForm.Password)
                }));

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code:  Attempted Value: {testForm.Password} Message: Password cannot be null or empty");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_UserNameNotFound_Should_ReturnNotFound()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync((UserEntity)null);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var actualResult = ((NotFoundObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4004");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("User could not be found");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"The user with user name '{testForm.Username}' could not be found.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_SignInFailureBecauseOf2FA_Should_ReturnUnAuthorized()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.TwoFactorRequired);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            var actualResult = ((UnauthorizedObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4001");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("An authentication error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"The user '{testUser.Email}' requires 2FA in order to procceed with logging in.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_SignInFailureBecauseOfNotBeingAllowed_Should_ReturnUnAuthorized()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.NotAllowed);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            var actualResult = ((UnauthorizedObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4001");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("An authentication error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"The user '{testUser.Email}' is not allowed to sign in. Please check an make sure your email has been validated.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_SignInFailureBecauseUserIsLockedOut_Should_ReturnUnAuthorized()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();
            testUser.LockoutEnd = DateTime.UtcNow.AddMinutes(5);

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            var actualResult = ((UnauthorizedObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4001");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("An authentication error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"User {testUser.UserName} is locked out. Please try again after {testUser.LockoutEnd.Value}.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_SignInFailureIncrementingLockoutSucceeded_Should_ReturnUnAuthorized()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);
            _mockUserManager.Mock
                .Setup(x => x.AccessFailedAsync(testUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            var actualResult = ((UnauthorizedObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4001");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("An authentication error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"The password provided for the user {testForm.Username} was not valid.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_Login_SignInFailureIncrementingLockoutFailed_Should_ReturnUnAuthorized()
        {
            // Assign
            var testForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult());
            _mockUserManager.Mock
                .Setup(x => x.FindByNameAsync(testForm.Username))
                .ReturnsAsync(testUser);
            _mockSignInManager.Mock
                .Setup(x => x.PasswordSignInAsync(
                    It.IsAny<UserEntity>(),
                    testForm.Password,
                    testForm.RememberMe,
                    It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);
            _mockUserManager.Mock
                .Setup(x => x.AccessFailedAsync(testUser))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError 
                {
                    Code = "TESTING123", 
                    Description = "A lockout failure occurred"
                }));

            // Act
            var result = await Sut.Login(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var actualResult = ((InternalServerErrorObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-5000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo(
                "A system level error has occurred while trying to fullfill the request");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                "Error Code: TESTING123 Message: A lockout failure occurred");
            _mockRepository.Verify();
        }

        #endregion

        #region VerifyEmail Tests

        [Test]
        public async Task Test_VerifyEmail_ValidForm_Should_ReturnNoContent()
        {
            // Assign
            var testForm = new VerifyEmailFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), testUser);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(testUser, testForm.Token), IdentityResult.Success);

            // Act
            var result = await Sut.VerifyEmail(testForm);

            // Assert 
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status204NoContent);
            var actualResult = (NoContentResult)result;
            result.Should().BeNull();
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_VerifyEmail_NullForm_Should_ReturnBadRequest()
        {
            // Act
            var result = await Sut.VerifyEmail(null);

            // Assert 
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("The verify email form provided was null");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_VerifyEmail_ValidationFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new VerifyEmailFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("email", "Email cannot be null or empty", testForm.Email)
                }));

            // Act
            var result = await Sut.VerifyEmail(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code:  Attempted Value: {testForm.Email} Message: Email cannot be null or empty");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_VerifyEmail_UserEmailNotFound_Should_ReturnNotFound()
        {
            // Assign
            var testForm = new VerifyEmailFormFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), (UserEntity)null);

            // Act
            var result = await Sut.VerifyEmail(testForm);

            // Assert 
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var actualResult = ((NotFoundObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4004");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("User could not be found");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo($"User not found with email '{testForm.Email}'");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_VerifyEmail_ErrorSettingAccountEmailToVerified_Should_ReturnInternalServerError()
        {
            // Assign
            var testForm = new VerifyEmailFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), testUser);
            _mockUserManager.Setup(
                x => x.ConfirmEmailAsync(testUser, testForm.Token), 
                IdentityResult.Failed(new IdentityError { Code = "ERR123", Description = "Somethign bad happened..." }));

            // Act
            var result = await Sut.VerifyEmail(testForm);

            // Assert 
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var actualResult = ((InternalServerErrorObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-5000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo(
                "A system level error has occurred while trying to fullfill the request");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                "Error Code: ERR123 Message: Somethign bad happened...");
            _mockRepository.Verify();
        }

        #endregion

        #region ForgotPassword Tests

        [Test]
        public async Task Test_ForgotPassword_ValidForm_Should_ReturnNoContent()
        {
            // Assign
            var testForm = new ForgotPasswordFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), testUser);

            // Act
            var result = await Sut.ForgotPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status204NoContent);
            var actualResult = (NoContentResult)result;
            result.Should().BeNull();
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ForgotPassword_NullForm_Should_ReturnNoContent()
        {
            // Act
            var result = await Sut.ForgotPassword(null);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("The forgot password form provided was null");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ForgotPassword_ValidationFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new ForgotPasswordFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("email", "Email cannot be null or empty", testForm.Email)
                }));

            // Act
            var result = await Sut.ForgotPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code:  Attempted Value: {testForm.Email} Message: Email cannot be null or empty");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ForgotPassword_UserEmailNotFound_Should_ReturnNoContent()
        {
            // Assign
            var testForm = new ForgotPasswordFormFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), (UserEntity)null);

            // Act
            var result = await Sut.ForgotPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var actualResult = ((NotFoundObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4004");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("User could not be found");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo($"User not found with email '{testForm.Email}'");
            _mockRepository.Verify();
        }

        #endregion

        #region ResetPassword Tests

        [Test]
        public async Task Test_ResetPassword_ValidForm_Should_ReturnOk()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), testUser);
            _mockUserManager.Setup(x => x.ResetPasswordAsync(testUser, testForm.Token, testForm.Password), IdentityResult.Success);

            // Act
            var result = await Sut.ResetPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status200OK);
            var actualResult = ((OkObjectResult)result).Value as ResultModel<string>;
            actualResult.Data.Should().BeEquivalentTo($"User '{testForm.Email}' password has been reset.");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ResetPassword_NullForm_Should_ReturnOk()
        {
            // Act
            var result = await Sut.ResetPassword(null);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("The reset password form provided was null");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ResetPassword_ValidationFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();

            _mockFormValidator
                .Setup(x => x.ValidateAsync(testForm))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("email", "Email cannot be null or empty", testForm.Email)
                }));

            // Act
            var result = await Sut.ResetPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo(
                $"Error Code:  Attempted Value: {testForm.Email} Message: Email cannot be null or empty");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ResetPassword_UserEmailNotFound_Should_ReturnNotFound()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), (UserEntity)null);

            // Act
            var result = await Sut.ResetPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var actualResult = ((NotFoundObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4004");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("User could not be found");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo($"User not found with email '{testForm.Email}'");
            _mockRepository.Verify();
        }

        [Test]
        public async Task Test_ResetPassword_ResetPasswordFailure_Should_ReturnUnProcessableEntity()
        {
            // Assign
            var testForm = new ResetPasswordFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();

            _mockUserManager.Setup(x => x.FindByEmailAsync(testForm.Email), testUser);
            _mockUserManager.Setup(
                x => x.ResetPasswordAsync(testUser, testForm.Token, testForm.Password), 
                IdentityResult.Failed(new IdentityError { Code = "ERR123", Description = "New password was not valid" }));

            // Act
            var result = await Sut.ResetPassword(testForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            var actualResult = ((UnprocessableEntityObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.ToArray()[0].Code.Should().BeEquivalentTo("MN-4220");
            actualResult.Errors.ToArray()[0].Message.Should().BeEquivalentTo("A validation error has occurred");
            actualResult.Errors.ToArray()[0].Detail.Should().BeEquivalentTo("Error Code: ERR123 Message: New password was not valid");
            _mockRepository.Verify();
        }

        #endregion
    }
}
