using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MovieNight.Data
{
    public class DesignTimeDbContextFactory :
        IDesignTimeDbContextFactory<AppDbContext>
    {
        public DesignTimeDbContextFactory()
        {

        }

        // Credit https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
        public AppDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                .AddJsonFile(Directory.GetCurrentDirectory() + "/appsettings.json")
                //.AddJsonFile(Directory.GetCurrentDirectory() + "/../MovieNight.Web/appsettings.Development.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            //builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new AppDbContext(builder.Options, null);
        }
    }
}
