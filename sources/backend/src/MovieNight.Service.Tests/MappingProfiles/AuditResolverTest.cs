using FluentAssertions;
using MovieNight.Common.Entities;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Service.MappingProfiles;
using MovieNight.Service.Models;
using NUnit.Framework;

namespace MovieNight.Service.Tests.MappingProfiles
{
    [TestFixture]
    internal class AuditResolverTest
    {
        [Test]
        public void Test_Resolve_ValidAuditInformation_Should_ReturnAuditModelMapped()
        {
            // Assign
            var testSource = new MovieEntityFaker().Generate();
            var testDestination = new MovieModel();

            var sut = new AuditResolver<MovieEntity, MovieModel>();

            // Act
            var result = sut.Resolve(testSource, testDestination, null, null);

            // Assert
            result.Should().NotBeNull();
            result.CreatedAt.Should().Be(testSource.CreatedAt);
            result.CreatedBy.Should().BeEquivalentTo(testSource.CreatedBy);
            result.UpdatedAt.Should().Be(testSource.UpdatedAt);
            result.UpdatedBy.Should().BeEquivalentTo(testSource.UpdatedBy);
        }

        [Test]
        public void Test_Resolve_NullAuditInformation_Should_ReturnAuditModelMappedWithDefaults()
        {
            // Assign
            var testDestination = new MovieModel();

            var sut = new AuditResolver<MovieEntity, MovieModel>();

            // Act
            var result = sut.Resolve(null, testDestination, null, null);

            // Assert
            result.Should().NotBeNull();
            result.CreatedAt.Should().Be(default);
            result.CreatedBy.Should().BeEquivalentTo(default);
            result.UpdatedAt.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }
    }
}
