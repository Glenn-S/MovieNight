using FluentValidation.Results;
using System.Threading.Tasks;

namespace MovieNight.Service.Validators
{
    /// <summary>
    /// Form validator for providing form validation relate operations.
    /// </summary>
    public interface IFormValidator
    {
        /// <summary>
        /// Validates a form of the passed in type.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="form"></param>
        /// <returns></returns>
        ValidationResult Validate<TForm>(TForm form)
            where TForm : class;

        /// <summary>
        /// Validates a form of the passed in type asynchronously.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<ValidationResult> ValidateAsync<TForm>(TForm form)
            where TForm : class;
    }
}
