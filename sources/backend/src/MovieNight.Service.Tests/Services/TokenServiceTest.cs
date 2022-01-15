using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using MovieNight.Common.Options;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Service.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MovieNight.Service.Tests.Services
{
    [TestFixture]
    public class TokenServiceTest
    {
        protected ITokenService Sut;

        private MockRepository _mockRepository;
        private ILogger<TokenService> _mockLogger;
        private Mock<IOptions<JwtSecretOptions>> _mockJwtSecretOptions;

        [SetUp]
        public void SetUp()
        {

            _mockRepository = new MockRepository(MockBehavior.Loose);

            _mockLogger = new NullLogger<TokenService>();
            _mockJwtSecretOptions = _mockRepository.Create<IOptions<JwtSecretOptions>>();
            _mockJwtSecretOptions
                .Setup(x => x.Value)
                .Returns(new JwtSecretOptionsFaker().Generate());

            Sut = new TokenService(
                _mockLogger,
                _mockJwtSecretOptions.Object);
        }

        #region Constructor Tests

        [Test]
        public void Test_Ctor_NullLogger_Should_ThrowArgumentNullException()
        {
            // Act
            Func<TokenService> result = () => new(null, _mockJwtSecretOptions.Object);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'logger')");
        }

        [Test]
        public void Test_Ctor_NullOptions_Should_ThrowArgumentNullException()
        {
            // Act
            Func<TokenService> result = () => new(_mockLogger, null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'jwtSecret')");
        }

        #endregion

        #region GetJwtToken Tests

        [Test]
        public void Test_GetJwtToken_ValidClaims_Should_ReturnNewJwtToken()
        {
            // Assign
            var testClaims = new List<Claim> 
            {
                new Claim("id", "true")
            };

            // Act
            var result = Sut.GetJwtToken(testClaims, DateTime.UtcNow.AddMinutes(15));

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test_GetJwtToken_NullClaims_Should_ThrowArgumentNullException()
        {
            // Act
            Func<string> result = () => Sut.GetJwtToken(null, DateTime.UtcNow.AddMinutes(15));

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'claims')");
        }

        [Test]
        public void Test_GetJwtToken_InvalidExpirationTime_Should_ThrowException()
        {
            // Assign
            var testClaims = new List<Claim>
            {
                new Claim("id", "true")
            };

            // Act
            Func<string> result = () => Sut.GetJwtToken(testClaims, DateTime.UtcNow.AddMinutes(-1));

            // Assert
            result.Should().ThrowExactly<ArgumentException>()
                .WithMessage("IDX12401: Expires: '[PII is hidden. For more details, see https://aka.ms/IdentityModel/PII.]' must be after NotBefore: '[PII is hidden. For more details, see https://aka.ms/IdentityModel/PII.]'.");
        }

        #endregion
    }
}
