using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieNight.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieNight.Data.Repositories
{
    // TODO add tests and fill out EditMovie
    internal class MovieRepository :
        IMovieRepository
    {
        private readonly ILogger<MovieRepository> _logger;
        private readonly IAppDbContext _context;

        public MovieRepository(
            ILogger<MovieRepository> logger,
            IAppDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MovieEntity> AddMovie(MovieEntity movieToAdd)
        {
            _logger.LogInformation("");
            var movieEntity = await _context.Movies.AddAsync(movieToAdd);
            await _context.SaveChangesAsync();

            return movieEntity.Entity;
        }

        public Task<MovieEntity> EditMovie(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<MovieEntity> GetMovie(Guid id)
        {
            return _context.Movies.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<MovieEntity>> GetMovies()
        {
            var movieEntities = await _context.Movies.ToListAsync(); // will not allow returning the task so having to do this...
            return movieEntities;
        }

        public async Task<MovieEntity> RemoveMovie(Guid id)
        {
            var movieToRemove = await _context.Movies.FindAsync(id);

            _context.Movies.Remove(movieToRemove);
            await _context.SaveChangesAsync();

            return movieToRemove;
        }
    }
}
