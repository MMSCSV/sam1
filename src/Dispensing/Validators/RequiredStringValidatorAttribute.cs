using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    public class RequiredStringValidatorAttribute : ValueValidatorAttribute
    {
        protected override Validator DoCreateValidator(System.Type targetType)
        {
            return new RequiredStringValidator(Negated);
        }
    }
}
