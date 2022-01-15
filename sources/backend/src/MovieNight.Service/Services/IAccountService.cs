using Microsoft.AspNetCore.Mvc;
using MovieNight.Service.Forms;
using System.Threading.Tasks;

namespace MovieNight.Service.Services
{
    /// <summary>
    /// Service for providing account related services.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Registers a new user in the application
        /// </summary>
        /// <remarks>
        /// An email to the users email will be sent to have them
        /// verify their address.
        /// </remarks>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<IActionResult> RegisterUser(RegistrationForm form);

        /// <summary>
        /// Signs a user in provided valid credentials.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<IActionResult> Login(LoginForm form);

        /// <summary>
        /// Verifies an email for a new account.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<IActionResult> VerifyEmail(VerifyEmailForm form);

        /// <summary>
        /// Sets the user with the email passed in with a 
        /// password reset.
        /// </summary>
        /// <remarks>
        /// The email, provided it is a valid email in the system,
        /// will be sent a password reset email.
        /// </remarks>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<IActionResult> ForgotPassword(ForgotPasswordForm form);

        /// <summary>
        /// Resets a users password.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<IActionResult> ResetPassword(ResetPasswordForm form);
    }
}
