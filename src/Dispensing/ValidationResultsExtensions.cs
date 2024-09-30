using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace CareFusion.Dispensing
{
    public static class ValidationResultsExtensions
    {
        public static IEnumerable<ValidationError> ToValidationErrorsArray(this ValidationResults validationResults)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            BuildValidationErrorsRecursive(validationErrors, validationResults);

            return validationErrors;
        }

        private static void BuildValidationErrorsRecursive(ICollection<ValidationError> validationErrors, IEnumerable<ValidationResult> results)
        {
            foreach (ValidationResult result in results)
            {
                Type target = null;
                if (result.Target != null)
                {
                    target = result.Target.GetType();
                }

                validationErrors.Add(new ValidationError(
                    target, 
                    result.Key, 
                    result.Message,
                    result.Tag));

                BuildValidationErrorsRecursive(validationErrors, result.NestedValidationResults);
            }
        }
    }
}
