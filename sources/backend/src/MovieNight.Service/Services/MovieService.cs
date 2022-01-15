using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieNight.Common.Entities;
using MovieNight.Data.Repositories;
using MovieNight.Service.Forms;
using MovieNight.Service.Models;
using System.Threading.Tasks;

namespace MovieNight.Service.Services
{
    // TODO add in tests and finish these methods up
    internal class MovieService :
        IMovieService
    {
        private readonly ILogger<MovieService> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieService(
            ILogger<MovieService> logger,
            IMovieRepository movieRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _movieRepository = movieRepository ?? throw new System.ArgumentNullException(nameof(movieRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<IActionResult> AddMovie(NewMovieForm form)
        {
            if (form is null)
            {

            }

            // validate the form

            // insert movie
            var movieToInsert = _mapper.Map<MovieEntity>(form);

            var result = await _movieRepository.AddMovie(movieToInsert);

            return new CreatedAtRouteResult(
                $"v1/movies/{result.Id}",
                _mapper.Map<MovieModel>(movieToInsert));
        }
    }
}
