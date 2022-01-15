using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieNight.Common.Results;
using MovieNight.Service.Forms;
using MovieNight.Service.Models;
using MovieNight.Service.Services;
using System.Threading.Tasks;

namespace MovieNight.Web.Controllers
{
    [ApiController]
    [Route("v1/accounts")]
    public class AccountsController :
        Controller
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService service)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Route("actions/register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> Register([FromBody] RegistrationForm form)
        {
            return _service.RegisterUser(form);
        }

        [HttpPost]
        [Route("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> VerifyEmail([FromBody] VerifyEmailForm form)
        {
            return _service.VerifyEmail(form);
        }

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TokenModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordForm form)
        {
            return _service.ForgotPassword(form);
        }

        [HttpPost]
        [Route("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> ResetPassword([FromBody] ResetPasswordForm form)
        {
            return _service.ResetPassword(form);
        }
    }
}
