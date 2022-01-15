using AutoMapper;
using MovieNight.Common.Entities;
using MovieNight.Service.Forms;
using MovieNight.Service.Models;

namespace MovieNight.Service.MappingProfiles
{
    /// <summary>
    /// Mapping profile for handling movie related mappings.
    /// </summary>
    internal class MovieMappingProfile :
        Profile
    {
        public MovieMappingProfile()
        {
            CreateMap<NewMovieForm, MovieEntity>();

            CreateMap<MovieEntity, MovieModel>()
                .ForMember(x => x.AuditInformation, opt => opt.MapFrom<AuditResolver<MovieEntity, MovieModel>>());
        }
    }
}
