using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieNight.Common.Entities;
using MovieNight.Common.Results;
using MovieNight.Service.Forms;
using MovieNight.Service.Validators;
using System;
using System.Threading.Tasks;

namespace MovieNight.Service.Services
{
    internal class AccountService :
        IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IFormValidator _formValidator;

        public AccountService(
            ILogger<AccountService> logger,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            IFormValidator formValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _formValidator = formValidator ?? throw new ArgumentNullException(nameof(formValidator));
        }

        public Task<IActionResult> RegisterUser(RegistrationForm form)
        {
            if (form is null)
            {
                _logger.LogWarning("The form provided was null");
                return Task.FromResult((IActionResult)new BadRequestObjectResult(new ErrorModel
                {
                    Errors = new [] { ErrorDetail.MN4000("The registration form provided was null.") }
                }));
            }

            return RegisterUserInternal(form);
        }

        public Task<IActionResult> Login(LoginForm form)
        {
            if (form is null)
            {
                _logger.LogWarning("The login form provided was null");
                return Task.FromResult((IActionResult)new BadRequestObjectResult(new ErrorModel
                {
                    Errors = new [] { ErrorDetail.MN4000("The login form provided was null.") }
                }));
            }

            return LoginInternal(form);
        }

        public Task<IActionResult> VerifyEmail(VerifyEmailForm form)
        {
            if (form is null)
            {
                _logger.LogWarning("The verify email form provided was null");
                return Task.FromResult((IActionResult)new BadRequestObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4000("The verify email form provided was null") }
                }));
            }

            return VerifyEmailInternal(form);
        }

        public Task<IActionResult> ForgotPassword(ForgotPasswordForm form)
        {
            if (form is null)
            {
                _logger.LogWarning("The forgot password form provided was null");
                return Task.FromResult((IActionResult)new BadRequestObjectResult(new ErrorModel 
                { 
                    Errors = new[] { ErrorDetail.MN4000("The forgot password form provided was null") } 
                }));
            }

            return ForgotPasswordInternal(form);
        }

        public Task<IActionResult> ResetPassword(ResetPasswordForm form)
        {
            if (form is null)
            {
                _logger.LogWarning("The reset password form provided was null");
                return Task.FromResult((IActionResult) new BadRequestObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4000("The reset password form provided was null") }
                }));
            }

            return ResetPasswordInternal(form);
        }

        private async Task<IActionResult> RegisterUserInternal(RegistrationForm form)
        {
            _logger.LogInformation("Registering user '{UserName}'", form.Username);

            // Validate the user form
            var validationResult = await _formValidator.ValidateAsync(form);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("The registration form was not valid");
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetValidationErrors(validationResult.Errors)
                });
            }

            // Create the new user
            _logger.LogDebug(
                "Assembling user entity with user name '{UserName}' and email '{Email}'...", 
                form.Username,
                form.Email);
            var user = new UserEntity
            {
                UserName = form.Username,
                Email = form.Email
            };

            _logger.LogInformation("Creating new user '{UserName}'", form.Username);
            var result = await _userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "There were errors while trying to process the new users details with username {UserName}",
                    form.Username);
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetIdentityErrors(result.Errors)
                });
            }

            // add new account to the database with link to AspNetUsers table

            // TODO Send verification email with token in it
            _logger.LogInformation("Getting email confirmation verification token");
            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); // send token in email

            _logger.LogDebug("Sending email...");

            // To return the user model later
            return new CreatedAtRouteResult(
                $"/v1/accounts/{user.Id}",
                new ResultModel<string> { Data = $"User with email '{form.Email}' has been created." });
        }

        private async Task<IActionResult> LoginInternal(LoginForm form)
        {
            _logger.LogInformation("Logging in user '{UserName}'", form.Username);
            var validationResult = await _formValidator.ValidateAsync(form);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("The login form was not valid");
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetValidationErrors(validationResult.Errors)
                });
            }

            _logger.LogInformation("Looking for user with user name '{UserName}'", form.Username);
            var user = await _userManager.FindByNameAsync(form.Username);
            if (user is null)
            {
                _logger.LogWarning("The user with user name '{UserName}' could not be found", form.Username);
                return new NotFoundObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4004($"The user with user name '{form.Username}' could not be found.") }
                });
            }

            _logger.LogDebug("Logging user '{Email}' in to the system...", user.Email);
            var signInResult = await _signInManager.PasswordSignInAsync(
                user,
                form.Password,
                form.RememberMe,
                false);

            if (!signInResult.Succeeded)
            {
                _logger.LogDebug("Authentication failed for user '{Email}'", user.Email);
                if (signInResult.RequiresTwoFactor)
                {
                    _logger.LogWarning("Current user '{Email}' must use 2FA to sign in", user.Email);
                    return new UnauthorizedObjectResult(new ErrorModel 
                    {
                        Errors = new[] { ErrorDetail.MN4001($"The user '{user.Email}' requires 2FA in order to procceed with logging in.") }
                    });
                }
                else if (signInResult.IsNotAllowed)
                {
                    _logger.LogWarning("Current user '{Email}' is not allowed to sign in", user.Email);
                    return new UnauthorizedObjectResult(new ErrorModel 
                    {
                        Errors = new[] { ErrorDetail.MN4001($"The user '{user.Email}' is not allowed to sign in. Please check an make sure your email has been validated.") }
                    });
                }
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("Current user '{Email}' is locked out", user.Email);
                    return new UnauthorizedObjectResult(new ErrorModel
                    {
                        Errors = new[]
                        {
                            ErrorDetail.MN4001($"User {user.UserName} is locked out. Please try again after {user.LockoutEnd.Value}.") // TODO get locale info to convert this not to utc
                        }
                    });
                }
                else
                {

                    var identityResult = await _userManager.AccessFailedAsync(user); // Access failed, increment the failed count
                    if (identityResult.Succeeded)
                    {
                        _logger.LogWarning("The password provided for the user {UserName} was not valid", form.Username);
                        return new UnauthorizedObjectResult(new ErrorModel
                        {
                            Errors = new[]
                            {
                                ErrorDetail.MN4001($"The password provided for the user {form.Username} was not valid.")
                            }
                        });
                    }

                    _logger.LogError(
                        "An error occurred while trying to increment the failed attempts for user '{Email}'",
                        user.Email);
                    return new InternalServerErrorObjectResult(new ErrorModel 
                    {
                        Errors = ErrorDetail.GetIdentityInternalErrors(identityResult.Errors)
                    });
                } 
            }

            return new OkObjectResult(user);
        }

        private async Task<IActionResult> VerifyEmailInternal(VerifyEmailForm form)
        {
            var validationResult = await _formValidator.ValidateAsync(form);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("The registration form was not valid");
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetValidationErrors(validationResult.Errors)
                });
            }

            // Email is guaranteed to be unique in the system so use it for finding the user
            _logger.LogInformation("Looking for user with email '{Email}'", form.Email);
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user is null)
            {
                // Return not found
                _logger.LogWarning("User could not be found");
                return new NotFoundObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4004($"User not found with email '{form.Email}'") }
                });
            }

            _logger.LogInformation("Confirming email verification token");
            var result = await _userManager.ConfirmEmailAsync(user, form.Token);
            if (!result.Succeeded)
            {
                _logger.LogWarning("There was an error trying to confirm the email address '{Email}'", form.Email);
                return new InternalServerErrorObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetIdentityInternalErrors(result.Errors)
                });
            }

            return new NoContentResult();
        }

        private async Task<IActionResult> ForgotPasswordInternal(ForgotPasswordForm form)
        {
            var validationResult = await _formValidator.ValidateAsync(form);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("The registration form was not valid");
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetValidationErrors(validationResult.Errors)
                });
            }

            _logger.LogInformation("Looking for user with email '{Email}'", form.Email);
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user is null)
            {
                // Return not found
                _logger.LogWarning("User could not be found");
                return new NotFoundObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4004($"User not found with email '{form.Email}'") }
                });
            }

            // TODO Send password reset email with token
            _logger.LogInformation("Generating password reset token");
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            _logger.LogDebug("Sending email...");

            return new NoContentResult();
        }

        private async Task<IActionResult> ResetPasswordInternal(ResetPasswordForm form)
        {
            var validationResult = await _formValidator.ValidateAsync(form);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("The registration form was not valid");
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetValidationErrors(validationResult.Errors)
                });
            }

            _logger.LogInformation("Looking for user with email '{Email}'", form.Email);
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user is null)
            {
                _logger.LogWarning("User could not be found");
                return new NotFoundObjectResult(new ErrorModel
                {
                    Errors = new[] { ErrorDetail.MN4004($"User not found with email '{form.Email}'") }
                });
            }

            _logger.LogInformation("Reseting user '{Email}' password", form.Email);
            var result = await _userManager.ResetPasswordAsync(user, form.Token, form.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("There was an issue trying to reset the password for the account with the email address '{Email}'", form.Email);
                return new UnprocessableEntityObjectResult(new ErrorModel
                {
                    Errors = ErrorDetail.GetIdentityErrors(result.Errors)
                });
            }

            _logger.LogInformation("User '{Email}' password has been reset", form.Email);
            return new OkObjectResult(new ResultModel<string> { Data = $"User '{form.Email}' password has been reset." });
        }
    }
}
