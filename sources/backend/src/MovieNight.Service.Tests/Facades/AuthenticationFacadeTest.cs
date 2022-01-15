using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MovieNight.Common.Results;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Service.Facades;
using MovieNight.Service.Models;
using MovieNight.Service.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieNight.Service.Tests.Facades
{
    [TestFixture]
    internal class AuthenticationFacadeTest
    {
        protected AuthenticationFacade Sut;

        private MockRepository _mockRepository;
        private ILogger<AuthenticationFacade> _mockLogger;
        private Mock<IAccountService> _mockAccountService;
        private Mock<ITokenService> _mockTokenService;
        private Mock<IMapper> _mockMapper;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);

            _mockLogger = new NullLogger<AuthenticationFacade>();
            _mockAccountService = new Mock<IAccountService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockMapper = new Mock<IMapper>();

            Sut = new(
                _mockLogger,
                _mockAccountService.Object,
                _mockTokenService.Object,
                _mockMapper.Object);
        }

        #region Constructor Tests

        [Test]
        public void Test_Ctor_NullLogger_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AuthenticationFacade> result = () => new AuthenticationFacade(
                null,
                _mockAccountService.Object,
                _mockTokenService.Object,
                _mockMapper.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'logger')");
        }

        [Test]
        public void Test_Ctor_NullAccountService_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AuthenticationFacade> result = () => new AuthenticationFacade(
                _mockLogger,
                null,
                _mockTokenService.Object,
                _mockMapper.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'accountService')");
        }

        [Test]
        public void Test_Ctor_NullTokenService_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AuthenticationFacade> result = () => new AuthenticationFacade(
                _mockLogger,
                _mockAccountService.Object,
                null,
                _mockMapper.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'tokenService')");
        }

        [Test]
        public void Test_Ctor_NullMapper_Should_ThrowArgumentNullException()
        {
            // Act
            Func<AuthenticationFacade> result = () => new AuthenticationFacade(
                _mockLogger,
                _mockAccountService.Object,
                _mockTokenService.Object,
                null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'mapper')");
        }

        #endregion

        #region Authenticate Tests

        [Test]
        public async Task Test_Authenticate_ValidLoginForm_Should_ReturnToken()
        {
            // Assign
            var testLoginForm = new LoginFormFaker().Generate();
            var testUser = new UserEntityFaker().Generate();
            var expectedToken = "some random jwt token";
            var testAccountResult = new OkObjectResult(testUser);

            _mockAccountService
                .Setup(x => x.Login(testLoginForm))
                .ReturnsAsync(testAccountResult);
            _mockTokenService
                .Setup(x => x.GetJwtToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<DateTime>()))
                .Returns(expectedToken);

            // Act
            var result = await Sut.Authenticate(testLoginForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status200OK);
            var actualResult = ((OkObjectResult)result).Value as TokenModel;
            actualResult.Should().NotBeNull();
            actualResult.Token.Should().NotBeNullOrEmpty();
            actualResult.Token.Should().BeEquivalentTo(expectedToken);
            _mockAccountService.Verify(x => x.Login(testLoginForm), Times.Once);
            _mockTokenService.Verify(x => x.GetJwtToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public async Task Test_Authenticate_IssueInAccountService_Should_ReturnNotReturnToken()
        {
            // Assign
            var testUser = new UserEntityFaker().Generate();
            var error = new ErrorModel
            {
                Errors = new[] { ErrorDetail.MN4000("Something naughty happened") }
            };
            var testAccountResult = new BadRequestObjectResult(error);
            var expectedResult = new BadRequestObjectResult(error);

            _mockAccountService
                .Setup(x => x.Login(null))
                .ReturnsAsync(testAccountResult);

            // Act
            var result = await Sut.Authenticate(null);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResult = ((BadRequestObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.First().Code.Should().BeEquivalentTo("MN-4000");
            actualResult.Errors.First().Message.Should().BeEquivalentTo("The form provided was null");
            actualResult.Errors.First().Detail.Should().BeEquivalentTo("Something naughty happened");
            _mockAccountService.Verify(x => x.Login(null), Times.Once);
        }

        [Test]
        public async Task Test_Authenticate_GetJwtTokenReturnedNull_Should_ReturnInternalServerError()
        {
            // Assign
            var testLoginForm = new LoginFormFaker().Generate();
            var testAccountResult = new OkObjectResult(new UserEntityFaker().Generate());

            _mockAccountService
                .Setup(x => x.Login(testLoginForm))
                .ReturnsAsync(testAccountResult);
            _mockTokenService
                .Setup(x => x.GetJwtToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<DateTime>()))
                .Returns((string)null);

            // Act
            var result = await Sut.Authenticate(testLoginForm);

            // Assert
            result.Should().NotBeNull();
            ((StatusCodeResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var actualResult = ((InternalServerErrorObjectResult)result).Value as ErrorModel;
            actualResult.Should().NotBeNull();
            actualResult.Errors.Should().HaveCount(1);
            actualResult.Errors.First().Code.Should().BeEquivalentTo("MN-5000");
            actualResult.Errors.First().Message.Should().BeEquivalentTo(
                "A system level error has occurred while to fullfill the request");
            actualResult.Errors.First().Detail.Should().BeEquivalentTo(
                $"There was an error generating the jwt token for the user {testLoginForm.Username}");
            _mockAccountService.Verify(x => x.Login(testLoginForm), Times.Once);
            _mockTokenService.Verify(x => x.GetJwtToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<DateTime>()), Times.Once);
        }

        #endregion
    }
}
