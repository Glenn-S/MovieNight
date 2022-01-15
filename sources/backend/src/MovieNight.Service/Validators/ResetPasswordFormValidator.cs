using FluentValidation;
using FluentValidation.Results;
using MovieNight.Service.Attributes;
using MovieNight.Service.Forms;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Validator for handling the <see cref="ResetPasswordForm"/> validation.
    /// </summary>
    [ValidatorType(typeof(ResetPasswordForm))]
    internal class ResetPasswordFormValidator :
        AbstractValidator<ResetPasswordForm>
    {
        public ResetPasswordFormValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("The form token cannot be null or empty");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be null or empty");
            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Confirm password cannot be null or empty")
                .Custom((confirmPassword, context) => 
                {
                    if (!(confirmPassword == context.InstanceToValidate.Password))
                    {
                        context.AddFailure(new ValidationFailure(
                            context.PropertyName,
                            "The confirmed password does not match the password provided",
                            confirmPassword));
                    }
                });
        }
    }
}
