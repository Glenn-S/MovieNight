using FluentAssertions;
using MovieNight.Common.Entities;
using MovieNight.Service.Forms;
using MovieNight.Service.Models;

namespace MovieNight.Common.Testing.Assertions
{
    public static partial class Assert
    {
        public static void AssertMovie(MovieEntity actual, NewMovieForm expected)
        {
            actual.Title.Should().BeEquivalentTo(expected.Title);
            actual.Description.Should().BeEquivalentTo(expected.Description);
            actual.Genre.Should().BeEquivalentTo(expected.Genre);
            actual.Director.Should().BeEquivalentTo(expected.Director);
            actual.Rating.Should().Be(expected.Rating);
            actual.FilmLength.Should().Be(expected.FilmLength);
            actual.ReleaseDate.Should().Be(expected.ReleaseDate);
        }

        public static void AssertMovie(MovieModel actual, MovieEntity expected)
        {
            actual.Id.Should().Be(expected.Id);
            actual.Title.Should().BeEquivalentTo(expected.Title);
            actual.Description.Should().BeEquivalentTo(expected.Description);
            actual.Genre.Should().BeEquivalentTo(expected.Genre);
            actual.Director.Should().BeEquivalentTo(expected.Director);
            actual.Rating.Should().Be(expected.Rating);
            actual.FilmLength.Should().Be(expected.FilmLength);
            actual.ReleaseDate.Should().Be(expected.ReleaseDate);
            actual.AuditInformation.CreatedAt.Should().Be(expected.CreatedAt);
            actual.AuditInformation.CreatedBy.Should().BeEquivalentTo(expected.CreatedBy);
            actual.AuditInformation.UpdatedAt.Should().Be(expected.UpdatedAt);
            actual.AuditInformation.UpdatedBy.Should().BeEquivalentTo(expected.UpdatedBy);
        }
    }
}
