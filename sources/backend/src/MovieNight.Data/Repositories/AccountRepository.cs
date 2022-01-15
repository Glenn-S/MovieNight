using Microsoft.Extensions.Logging;
using MovieNight.Common.Entities;
using System;
using System.Threading.Tasks;

namespace MovieNight.Data.Repositories
{
    internal class AccountRepository :
        IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger;
        private readonly IAppDbContext _context;

        public AccountRepository(
            ILogger<AccountRepository> logger,
            IAppDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RefreshTokenEntity> AddRefreshToken(RefreshTokenEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            var insertedEntity = await _context.RefreshTokens.AddAsync(entity);
            await _context.SaveChangesAsync();
            return insertedEntity.Entity;
        }
    }
}
