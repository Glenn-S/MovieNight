using Microsoft.AspNetCore.Http;
using MovieNight.Common.Entities;

namespace MovieNight.Common.Resolvers
{
    /// <summary>
    /// User resolver for handling getting the current application user
    /// from the <see cref="HttpContext"/>.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-6.0</remarks>
    public class UserResolver :
        IUserResolver
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserResolver(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public UserEntity GetUser() 
        {
            var user = _contextAccessor.HttpContext?.Items["User"];
            return user as UserEntity;
        }
    }
}
