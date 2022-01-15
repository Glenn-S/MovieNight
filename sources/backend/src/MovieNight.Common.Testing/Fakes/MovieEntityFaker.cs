using Bogus;
using MovieNight.Common.Entities;
using System;

namespace MovieNight.Common.Testing.Fakes
{
    public class MovieEntityFaker :
        Faker<MovieEntity>
    {
        private Guid? _id;
        private string _title;
        private string _description;
        private string _genre;
        private string _director;
        private decimal? _rating;
        private TimeSpan? _filmLength;
        private DateTime? _releaseDate;

        public MovieEntityFaker()
        {
            RuleFor(x => x.Id, f => _id ?? f.Random.Guid());
            RuleFor(x => x.Title, f => _title ?? f.Lorem.Sentence(5));
            RuleFor(x => x.Description, f => _description ?? f.Lorem.Sentence());
            RuleFor(x => x.Genre, f => _genre ?? f.Lorem.Word());
            RuleFor(x => x.Director, f => _director ?? f.Person.FullName);
            RuleFor(x => x.Rating, f => _rating ?? f.Random.Decimal(0.0m, 10.0m));
            RuleFor(x => x.FilmLength, f => _filmLength ?? new TimeSpan(2, 4, 0));
            RuleFor(x => x.ReleaseDate, f => _releaseDate ?? f.Date.Past());
        }

        public MovieEntityFaker WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public MovieEntityFaker WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public MovieEntityFaker WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public MovieEntityFaker WithGenre(string genre)
        {
            _genre = genre;
            return this;
        }

        public MovieEntityFaker WithDirector(string director)
        {
            _director = director;
            return this;
        }

        public MovieEntityFaker WithRating(decimal rating)
        {
            _rating = rating;
            return this;
        }

        public MovieEntityFaker WithFilmLength(TimeSpan filmLength)
        {
            _filmLength = filmLength;
            return this;
        }

        public MovieEntityFaker WithReleaseDate(DateTime releaseDate)
        {
            _releaseDate = releaseDate;
            return this;
        }
    }
}
