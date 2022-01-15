using FluentValidation;
using FluentValidation.Results;
using MovieNight.Service.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieNight.Service.Validators
{
    internal class FormValidator :
        IFormValidator
    {
        private IDictionary<Type, IValidator> _validators;
        public FormValidator(IEnumerable<IValidator> validators)
        {
            if (validators is null) throw new ArgumentNullException(nameof(validators));

            // get the attributes tagging the validators
            _validators = new Dictionary<Type, IValidator>();

            foreach (var validator in validators)
            {
                var validatorType = validator.GetType();
                var validatorFormType = validatorType.GetCustomAttribute<ValidatorTypeAttribute>()?.ValidatorFormType;

                if (validatorFormType is null)
                {
                    throw new ArgumentException($"The validator '{validatorType.Name}' did not have a validator type associated with it");
                }

                _validators.Add(validatorFormType, validator);
            }
        }

        public ValidationResult Validate<TForm>(TForm form)
            where TForm : class
        {
            var validator = GetValidator(form.GetType());
            return validator?.Validate(new ValidationContext<TForm>(form));
        }

        public Task<ValidationResult> ValidateAsync<TForm>(TForm form)
            where TForm : class
        {
            var validator = GetValidator(form.GetType());
            return validator.ValidateAsync(new ValidationContext<TForm>(form));
        }

        private IValidator GetValidator(Type formType)
        {
            if (!_validators.ContainsKey(formType))
            {
                throw new KeyNotFoundException($"A validator for the form type '{formType.Name}' does not exist");
            }

            return _validators[formType];
        }
    }
}
