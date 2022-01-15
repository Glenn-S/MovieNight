using System;

namespace MovieNight.Service.Attributes
{
    // TODO add in attribute tests
    [AttributeUsage(AttributeTargets.Class)]
    internal class ValidatorTypeAttribute :
        Attribute
    {
        public readonly Type ValidatorFormType;

        public ValidatorTypeAttribute(Type validatorFormType)
        {
            ValidatorFormType = validatorFormType ?? throw new ArgumentNullException(nameof(validatorFormType));
        }
    }
}
