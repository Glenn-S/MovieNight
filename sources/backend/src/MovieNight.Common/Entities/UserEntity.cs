using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MovieNight.Common.Entities
{
    public class UserEntity :
        IdentityUser
    {
        public virtual IEnumerable<RefreshTokenEntity> RefreshTokens { get; set; }
    }
}
