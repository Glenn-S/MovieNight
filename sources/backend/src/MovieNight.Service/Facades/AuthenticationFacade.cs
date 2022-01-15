using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieNight.Common.Entities;
using MovieNight.Common.Results;
using MovieNight.Service.Forms;
using MovieNight.Service.Models;
using MovieNight.Service.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieNight.Service.Facades
{
    internal class AuthenticationFacade :
        IAuthenticationFacade
    {
        private readonly ILogger<AuthenticationFacade> _logger;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthenticationFacade(
            ILogger<AuthenticationFacade> logger,
            IAccountService accountService,
            ITokenService tokenService,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IActionResult> Authenticate(LoginForm form)
        {
            var loginResult = await _accountService.Login(form);
            var statusCode = ((StatusCodeResult)loginResult).StatusCode;

            // Write mapper to translate results
            if (statusCode != StatusCodes.Status200OK)
            {
                // Return the error result
                return loginResult;
            }

            // TODO Handle getting the users claims. This is temporary for now
            var data = ((OkObjectResult)loginResult).Value as UserEntity;
            var claims = new Claim[] { new("id", data?.Id.ToString()) };
            var jwtToken = _tokenService.GetJwtToken(claims, DateTime.UtcNow.AddMinutes(15)); // Move to app settings
            if (jwtToken is null)
            {
                _logger.LogWarning("There was an error generating the jwt token");
                return new InternalServerErrorObjectResult(new ErrorModel
                {
                    Errors = new[]
                    {
                        ErrorDetail.MN5000($"There was an error generating the jwt token for the user {form.Username}")
                    }
                });
            }

            return new OkObjectResult(new TokenModel { Token = jwtToken });
        }
    }
}
