using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieNight.Common.Entities;
using MovieNight.Common.Resolvers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieNight.Data
{
    public class AppDbContext :
        IdentityDbContext<UserEntity>,
        IAppDbContext
    {
        private readonly IUserResolver _userResolver;

        public DbSet<MovieEntity> Movies { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IUserResolver userResolver) : 
            base(options)
        {
            _userResolver = userResolver;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesInternal();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SaveChangesInternal();
            return base.SaveChanges();
        }

        // inspiration https://stackoverflow.com/questions/3879011/entity-framework-sql2008-how-to-automatically-update-lastmodified-fields-for-e
        private void SaveChangesInternal()
        {
            foreach (var entity in ChangeTracker.Entries<IAudit>())
            {
                if (entity.State == EntityState.Added || entity.State == EntityState.Modified)
                {
                    var userName = _userResolver?.GetUser()?.UserName;

                    var currentTime = DateTime.UtcNow;
                    entity.Entity.UpdatedAt = currentTime;
                    entity.Entity.UpdatedBy = userName ?? "Anonymous"; // get from http context

                    if (entity.State == EntityState.Added)
                    {
                        entity.Entity.CreatedAt = currentTime;
                        entity.Entity.CreatedBy = userName ?? "Anonymous"; // get from http context
                    }
                }
            }
        }
    }
}
