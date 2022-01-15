using AutoMapper;
using MovieNight.Common.Entities;
using MovieNight.Service.Models;

namespace MovieNight.Service.MappingProfiles
{
    /// <summary>
    /// Auto mapper profile resolver for mapping an object that implements
    /// the <see cref="IAudit"/> interface and a destination object that
    /// implements the <see cref="IAuditModel"/>.
    /// </summary>
    /// <typeparam name="TSrc"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    public class AuditResolver<TSrc, TDest> :
            IValueResolver<TSrc, TDest, AuditModel>
            where TSrc : IAudit
            where TDest : IAuditModel
    {
        public AuditModel Resolve(TSrc source, TDest destination, AuditModel destMember, ResolutionContext context)
        {
            return new AuditModel
            {
                CreatedAt = source?.CreatedAt ?? default,
                CreatedBy = source?.CreatedBy,
                UpdatedAt = source?.UpdatedAt,
                UpdatedBy = source?.UpdatedBy
            };
        }
    }
}
