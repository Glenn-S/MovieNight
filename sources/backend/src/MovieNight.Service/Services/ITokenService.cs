using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MovieNight.Service.Services
{
    /// <summary>
    /// Service for handling token related operations.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gets a Jwt Token and applies the provided claims and 
        /// expiration date.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtSecret"></param>
        /// <param name="expiration"></param>
        /// <returns>
        /// Valid Jwt token signed by the application if successful.
        /// </returns>
        public string GetJwtToken(
            IEnumerable<Claim> claims,
            DateTime expiration);
    }
}
