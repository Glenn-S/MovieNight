using FluentValidation;
using MovieNight.Service.Attributes;
using MovieNight.Service.Forms;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Validator for handling the <see cref="RegistrationForm"/> validation.
    /// </summary>
    [ValidatorType(typeof(RegistrationForm))]
    internal class RegistrationFormValidator :
        AbstractValidator<RegistrationForm>
    {
        public RegistrationFormValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("User name cannot be null or empty");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be null or empty");
        }
    }
}
