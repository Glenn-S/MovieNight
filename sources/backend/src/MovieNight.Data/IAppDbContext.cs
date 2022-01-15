using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MovieNight.Common.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MovieNight.Data
{
    public interface IAppDbContext
    {
        DbSet<MovieEntity> Movies { get; set; }
        DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
