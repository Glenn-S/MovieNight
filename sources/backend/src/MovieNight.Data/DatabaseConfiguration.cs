using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieNight.Common.Entities;
using MovieNight.Data.Repositories;
using System;

namespace MovieNight.Data.Bootstrap
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                //config.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                config.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<IAppDbContext, AppDbContext>();
            services.AddRepositories();

            services.AddIdentity<UserEntity, IdentityRole>(config =>
            {
                // Password configuration
                config.Password.RequireDigit = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireUppercase = true;
                config.Password.RequiredLength = 10;
                config.Password.RequireNonAlphanumeric = true;

                // Lockout configuration
                config.Lockout.MaxFailedAccessAttempts = 5;
                config.Lockout.AllowedForNewUsers = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // Signin configuration
                config.SignIn.RequireConfirmedEmail = true;
                config.SignIn.RequireConfirmedAccount = false;
                config.SignIn.RequireConfirmedPhoneNumber = false;

                // User configuration
                config.User.RequireUniqueEmail = true;
                // config.User.AllowedUserNameCharacters = ""; // Using default for now
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
        }
    }
}
