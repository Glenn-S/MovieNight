using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MovieNight.Common.Testing.Mocks
{
    public class MockSignInManager<TUser>
        where TUser : class
    {
        private MockRepository _mockRepository;
        private Mock<SignInManager<TUser>> _mockSignInManager;

        public MockSignInManager()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);

            _mockSignInManager = _mockRepository.Create<SignInManager<TUser>>(
                new Mock<UserManager<TUser>>(
                    new Mock<IUserStore<TUser>>().Object, null, null, null, null, null, null, null, null).Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new NullLogger<SignInManager<TUser>>(),
                new Mock<IAuthenticationSchemeProvider>().Object);
        }

        public Mock<SignInManager<TUser>> Mock => _mockSignInManager;
        public SignInManager<TUser> Object => _mockSignInManager.Object;

        public void Verify() => _mockRepository.Verify();

        public MockSignInManager<TUser> Setup<TReturn>(
            Expression<Func<SignInManager<TUser>, TReturn>> setup,
            TReturn returns)
            where TReturn : class
        {
            _mockSignInManager.Setup(setup).Returns(returns);
            return this;
        }

        public MockSignInManager<TUser> Setup<TReturn>(
            Expression<Func<SignInManager<TUser>, Task<TReturn>>> setup,
            TReturn returns)
            where TReturn : class
        {
            _mockSignInManager.Setup(setup).ReturnsAsync(returns);
            return this;
        }
    }
}
