using CareFusion.Dispensing.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    public class RequiredStringValidator : ValueValidator<string>
    {
        public RequiredStringValidator()
            : this(false)
        {
            
        }
        
        public RequiredStringValidator(bool negated)
            : this(negated, null)
        {

        }
        
        public RequiredStringValidator(string messageTemplate)
            : this(false, messageTemplate)
        {

        }

        public RequiredStringValidator(bool negated, string messageTemplate)
            : base(messageTemplate, null, negated)
        {
        }

        protected override string DefaultNegatedMessageTemplate
        {
            get { return DispensingResources.RequiredStringValidatorNegatedDefaultMessageTemplate; }
        }

        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return DispensingResources.RequiredStringValidatorNonNegatedDefaultMessageTemplate; }
        }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (string.IsNullOrEmpty(objectToValidate) != Negated)
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }
        }
    }
}
