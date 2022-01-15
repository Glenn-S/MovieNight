using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieNight.Common.Entities;
using MovieNight.Common.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Credit: https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
namespace MovieNight.Web.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;
        private readonly JwtSecretOptions _jwtSecret;

        public JwtMiddleware(
            RequestDelegate next,
            ILogger<JwtMiddleware> logger,
            IOptions<JwtSecretOptions> jwtSecret)
        {
            if (jwtSecret is null) throw new ArgumentNullException(nameof(jwtSecret));

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jwtSecret = jwtSecret.Value;
        }

        public async Task Invoke(
            HttpContext context,
            UserManager<UserEntity> userManager)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split().Last();

            if (token is not null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_jwtSecret.Key);

                    tokenHandler.ValidateToken(
                        token,
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = _jwtSecret.Issuer,
                            ValidAudience = _jwtSecret.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ClockSkew = TimeSpan.Zero
                        },
                        out SecurityToken validatedToken);

                    var jwtToken = validatedToken as JwtSecurityToken;
                    if (jwtToken is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                    var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                    // Token has been validated and parsed so set the user for the session
                    context.Items["User"] = await userManager.FindByIdAsync(userId);
                }
                catch (Exception ex)
                {
                    // Continue
                    _logger.LogError(
                        ex,
                        "An error occurred while trying to validate the jwt token. (Message: {Message})",
                        ex.Message);
                }
            }

            await _next(context);
        }
    }
}
