using MovieNight.Common.Entities;
using System.Threading.Tasks;

namespace MovieNight.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<RefreshTokenEntity> AddRefreshToken(RefreshTokenEntity entity);
    }
}
