using Bogus;
using MovieNight.Service.Forms;
using System;

namespace MovieNight.Common.Testing.Fakes
{
    public class NewMovieFormFaker :
        Faker<NewMovieForm>
    {
        private string _title;
        private string _description;
        private string _genre;
        private string _director;
        private decimal? _rating;
        private TimeSpan? _filmLength;
        private DateTime? _releaseDate;

        public NewMovieFormFaker()
        {
            RuleFor(x => x.Title, f => _title ?? f.Lorem.Sentence(5));
            RuleFor(x => x.Description, f => _description ?? f.Lorem.Sentence());
            RuleFor(x => x.Genre, f => _genre ?? f.Lorem.Word());
            RuleFor(x => x.Director, f => _director ?? f.Person.FullName);
            RuleFor(x => x.Rating, f => _rating ?? f.Random.Decimal(0.0m, 10.0m));
            RuleFor(x => x.FilmLength, f => _filmLength ?? new TimeSpan(2, 4, 0));
            RuleFor(x => x.ReleaseDate, f => _releaseDate ?? f.Date.Past());
        }

        public NewMovieFormFaker WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public NewMovieFormFaker WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public NewMovieFormFaker WithGenre(string genre)
        {
            _genre = genre;
            return this;
        }

        public NewMovieFormFaker WithDirector(string director)
        {
            _director = director;
            return this;
        }

        public NewMovieFormFaker WithRating(decimal rating)
        {
            _rating = rating;
            return this;
        }

        public NewMovieFormFaker WithFilmLength(TimeSpan filmLength)
        {
            _filmLength = filmLength;
            return this;
        }

        public NewMovieFormFaker WithReleaseDate(DateTime releaseDate)
        {
            _releaseDate = releaseDate;
            return this;
        }
    }
}
