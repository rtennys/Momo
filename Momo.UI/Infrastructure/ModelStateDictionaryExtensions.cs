using System;
using System.Linq;
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

        public static string[] ToErrorList(this ModelStateDictionary modelState)
        {
            return modelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)
                                 ? x.ErrorMessage
                                 : x.Exception != null
                                       ? x.Exception.Message
                                       : null)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .DefaultIfEmpty("Unknown Error")
                .ToArray();
        }
    }
}
