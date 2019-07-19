using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Momo.Domain
{
    public interface IValidationFacade
    {
        DomainResult Validate(object instance);
    }

    public class ValidationFacade : IValidationFacade
    {
        public DomainResult Validate(object instance)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

            var result = new DomainResult();

            foreach (var validationResult in validationResults)
                result.Add(validationResult.MemberNames.Join(", "), validationResult.ErrorMessage);

            return result;
        }
    }
}
