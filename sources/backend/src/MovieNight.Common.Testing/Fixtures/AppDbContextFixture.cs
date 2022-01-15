using Microsoft.EntityFrameworkCore;
using MovieNight.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MovieNight.Common.Testing.Fixtures
{
    public class AppDbContextFixture
    {
        protected IAppDbContext Context;

        public AppDbContextFixture()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb");
            Context = new AppDbContext(dbContextOptions.Options, null);
        }

        [SetUp]
        public async Task SetupDatabase()
        {
            await Context.Database.EnsureCreatedAsync();
        }

        [TearDown]
        public async Task TearDownDatabase()
        {
            await Context.Database.EnsureDeletedAsync();
        }
    }
}
