using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieNight.Common.Results;
using MovieNight.Service.Facades;
using MovieNight.Service.Forms;
using System.Threading.Tasks;

namespace MovieNight.Web.Controllers
{
    [ApiController]
    [Route("v1/auth")]
    public class AuthenticationController
    {
        private readonly IAuthenticationFacade _facade;

        public AuthenticationController(IAuthenticationFacade facade)
        {
            _facade = facade ?? throw new System.ArgumentNullException(nameof(facade));
        }

        [HttpPost]
        [Route("actions/login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))] // returns a token
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> Login([FromBody] LoginForm form)
        {
            return _facade.Authenticate(form);
        }
    }
}
