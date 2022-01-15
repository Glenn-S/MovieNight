using FluentValidation;
using MovieNight.Service.Attributes;
using MovieNight.Service.Forms;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Validator for handling the <see cref="VerifyEmailForm"/> validation.
    /// </summary>
    [ValidatorType(typeof(VerifyEmailForm))]
    internal class VerifyEmailFormValidator :
        AbstractValidator<VerifyEmailForm>
    {
        public VerifyEmailFormValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("The form token cannot be null or empty");
        }
    }
}
