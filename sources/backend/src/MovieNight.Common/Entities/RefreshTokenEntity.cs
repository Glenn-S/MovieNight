using System;

namespace MovieNight.Common.Entities
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string RevokedByIp { get; set; }
    }
}
