using Microsoft.AspNetCore.Mvc;
using MovieNight.Service.Forms;
using System.Threading.Tasks;

namespace MovieNight.Service.Facades
{
    /// <summary>
    /// A facade for handling authentication related operations responsible
    /// for logging a user in and providing tokens.
    /// </summary>
    public interface IAuthenticationFacade
    {
        /// <summary>
        /// Authenticates a user and provides a Jwt token if successful.
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// A Jwt token if successful.
        /// </returns>
        public Task<IActionResult> Authenticate(LoginForm form);
    }
}
