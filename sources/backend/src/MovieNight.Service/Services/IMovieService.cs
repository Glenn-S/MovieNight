using Microsoft.AspNetCore.Mvc;
using MovieNight.Service.Forms;
using System.Threading.Tasks;

namespace MovieNight.Service.Services
{
    public interface IMovieService
    {
        Task<IActionResult> AddMovie(NewMovieForm form);
    }
}
