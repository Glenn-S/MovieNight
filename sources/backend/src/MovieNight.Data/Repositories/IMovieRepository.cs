using MovieNight.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieNight.Data.Repositories
{
    public interface IMovieRepository
    {
        Task<MovieEntity> AddMovie(MovieEntity movieToAdd);
        Task<MovieEntity> GetMovie(Guid id);
        Task<IEnumerable<MovieEntity>> GetMovies(); // make this paged and filterable
        Task<MovieEntity> EditMovie(Guid id);
        Task<MovieEntity> RemoveMovie(Guid id);
    }
}
