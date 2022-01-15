using FluentValidation;
using MovieNight.Service.Attributes;
using MovieNight.Service.Forms;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Validator for handling the <see cref="ForgotPasswordForm"/> validation.
    /// </summary>
    [ValidatorType(typeof(ForgotPasswordForm))]
    internal class ForgotPasswordFormValidator :
        AbstractValidator<ForgotPasswordForm>
    {
        public ForgotPasswordFormValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be null or empty");
        }
    }
}
