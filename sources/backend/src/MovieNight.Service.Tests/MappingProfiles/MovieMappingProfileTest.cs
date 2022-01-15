using AutoMapper;
using FluentAssertions;
using MovieNight.Common.Entities;
using MovieNight.Common.Testing.Fakes;
using MovieNight.Service.Forms;
using MovieNight.Service.MappingProfiles;
using MovieNight.Service.Models;
using NUnit.Framework;
using System;
using Assert = MovieNight.Common.Testing.Assertions.Assert;

namespace MovieNight.Service.Tests.MappingProfiles
{
    [TestFixture]
    internal class MovieMappingProfileTest
    {
        private IMapper Sut;

        [SetUp]
        public void SetUp()
        {
            Sut = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MovieMappingProfile>();
            }).CreateMapper();
        }

        [Test]
        public void Test_Map_ValidNewMovieForm_Should_ReturnMovieEntity()
        {
            // Assign
            var testForm = new NewMovieFormFaker().Generate();

            // Act
            var result = Sut.Map<MovieEntity>(testForm);

            // Assert
            Assert.AssertMovie(result, testForm);
            result.CreatedAt.Should().Be(default);
            result.CreatedBy.Should().BeEquivalentTo(default);
            result.UpdatedAt.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }

        [Test]
        public void Test_Map_NullNewMovieForm_Should_ReturnNull()
        {
            // Act
            var result = Sut.Map<MovieEntity>((NewMovieForm)null);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Test_Map_ValidMovieEntity_Should_ReturnMovieModel()
        {
            // Assign
            var testEntity = new MovieEntityFaker().Generate();
            var time = DateTime.UtcNow;
            testEntity.CreatedAt = time.AddDays(-5);
            testEntity.CreatedBy = "Anonymous";
            testEntity.UpdatedAt = time.AddDays(-3);
            testEntity.UpdatedBy = "Anonymous";

            // Act
            var result = Sut.Map<MovieModel>(testEntity);

            // Assert
            Assert.AssertMovie(result, testEntity);
            result.AuditInformation.CreatedAt.Should().Be(testEntity.CreatedAt);
            result.AuditInformation.CreatedBy.Should().BeEquivalentTo(testEntity.CreatedBy);
            result.AuditInformation.UpdatedAt.Should().Be(testEntity.UpdatedAt);
            result.AuditInformation.UpdatedBy.Should().BeEquivalentTo(testEntity.UpdatedBy);
        }

        [Test]
        public void Test_Map_NullMovieEntity_Should_ReturnNull()
        {
            // Act
            var result = Sut.Map<MovieModel>((MovieEntity)null);

            // Assert
            result.Should().BeNull();
        }
    }
}
