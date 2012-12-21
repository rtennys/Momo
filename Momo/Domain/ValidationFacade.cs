using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Momo.Common;

namespace Momo.Domain
{
    public interface IValidationFacade
    {
        CommandResult Validate(object instance);
    }

    public class ValidationFacade : IValidationFacade
    {
        public CommandResult Validate(object instance)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

            var serviceResult = new CommandResult();

            foreach (var validationResult in validationResults)
                serviceResult.Add(validationResult.MemberNames.Join(", "), validationResult.ErrorMessage);

            return serviceResult;
        }
    }
}
