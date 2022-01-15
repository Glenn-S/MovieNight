using FluentValidation;
using MovieNight.Service.Attributes;
using MovieNight.Service.Forms;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Validator for handling the <see cref="LoginForm"/> validation.
    /// </summary>
    [ValidatorType(typeof(LoginForm))]
    internal class LoginFormValidator :
        AbstractValidator<LoginForm>
    {
        public LoginFormValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("User name cannot be null or empty");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be null or empty");
        }
    }
}
