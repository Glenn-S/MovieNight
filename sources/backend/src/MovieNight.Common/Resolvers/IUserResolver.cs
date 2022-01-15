using MovieNight.Common.Entities;

namespace MovieNight.Common.Resolvers
{
    /// <summary>
    /// Interface for handling the user context for the 
    /// request lifespan.
    /// </summary>
    public interface IUserResolver
    {
        /// <summary>
        /// Retrieves a user that is currently in the application context.
        /// </summary>
        /// <returns>
        /// A user if one has been authenticated and logged in.
        /// </returns>
        UserEntity GetUser();
    }
}
