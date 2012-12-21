using System;
using System.Web.Mvc;
using Momo.Domain;

namespace Momo.UI
{
    public static class ModelStateDictionaryExtensions
    {
        public static ModelStateDictionary AddModelErrors(this ModelStateDictionary modelState, CommandResult result)
        {
            foreach (var error in result.Errors)
                modelState.AddModelError(error.Key, error.Error);

            return modelState;
        }
    }
}
