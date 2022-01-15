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
    [Route("v1/movies")]
    public class MoviesController :
        Controller
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Authorize] // Should be Admin
        [Route("", Name = nameof(AddMovie))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MovieModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public Task<IActionResult> AddMovie([FromBody] NewMovieForm form)
        {
            return _service.AddMovie(form);
        }

        [HttpGet]
        [Authorize]
        [Route("", Name = nameof(GetMovies))]
        public IActionResult GetMovies()
        {
            var userClaims = User.Claims;
            return new OkResult();
        }
    }
}
