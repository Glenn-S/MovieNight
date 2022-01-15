using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

// mock fix from https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
namespace MovieNight.Common.Testing.Mocks
{
    public class MockUserManager<TUser>
        where TUser : class
    {
        private Mock<UserManager<TUser>> _mockUserManager;

        public MockUserManager()
        {
            var mockUserStore = new Mock<IUserStore<TUser>>();
            _mockUserManager = new Mock<UserManager<TUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            _mockUserManager.Object.UserValidators.Add(new UserValidator<TUser>());
            _mockUserManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        }

        public Mock<UserManager<TUser>> Mock => _mockUserManager;
        public UserManager<TUser> Object => _mockUserManager.Object;

        public MockUserManager<TUser> Setup<TReturn>(
            Expression<Func<UserManager<TUser>, TReturn>> setup,
            TReturn returns)
            where TReturn : class
        {
            _mockUserManager.Setup(setup).Returns(returns);
            return this;
        }

        public MockUserManager<TUser> Setup<TReturn>(
            Expression<Func<UserManager<TUser>, Task<TReturn>>> setup,
            TReturn returns)
            where TReturn : class
        {
            _mockUserManager.Setup(setup).ReturnsAsync(returns);
            return this;
        }
    }
}
