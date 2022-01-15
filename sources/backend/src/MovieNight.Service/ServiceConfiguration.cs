using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MovieNight.Service.Facades;
using MovieNight.Service.Services;
using MovieNight.Service.Validators;

namespace MovieNight.Service.Bootstrap
{
    public static class ServiceConfiguration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddValidators();

            services.AddAutoMapper(typeof(ServiceConfiguration).Assembly);

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMovieService, MovieService>();

            services.AddScoped<IAuthenticationFacade, AuthenticationFacade>();
        }

        private static void AddValidators(this IServiceCollection services)
        {
            services.Scan(scan =>
            {
                scan.FromAssemblies(typeof(ServiceConfiguration).Assembly)
                    .AddClasses(x => x.AssignableTo<IValidator>())
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime();
            });

            services.AddScoped<IFormValidator, FormValidator>();
        }
    }
}
