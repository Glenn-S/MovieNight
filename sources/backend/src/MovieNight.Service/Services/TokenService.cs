using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieNight.Common.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieNight.Service.Services
{
    internal class TokenService :
        ITokenService
    {
        private readonly JwtSecretOptions _jwtSecret;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            ILogger<TokenService> logger,
            IOptions<JwtSecretOptions> jwtSecret)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (jwtSecret is null) throw new ArgumentNullException(nameof(jwtSecret));
            _jwtSecret = jwtSecret.Value;
            _logger = logger;
        }

        public string GetJwtToken(
            IEnumerable<Claim> claims,
            DateTime expiration)
        {
            if (claims is null) throw new ArgumentNullException(nameof(claims));

            // TODO handle this setup more gracefully
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _jwtSecret.Issuer,
                Audience = _jwtSecret.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret.Key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // TODO add in refresh token logic later
    }
}
