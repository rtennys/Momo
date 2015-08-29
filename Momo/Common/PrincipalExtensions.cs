using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Momo.Common
{
    public static class PrincipalExtensions
    {
        public static string Get(this IPrincipal principal, string claimType)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null) return "";

            var claim = claimsPrincipal.FindFirst(claimType);

            return claim == null ? "" : claim.Value;
        }

        public static T Get<T>(this IPrincipal principal, string claimType)
        {
            var stringValue = principal.Get(claimType);

            if (string.IsNullOrWhiteSpace(stringValue)) return default(T);

            try
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(stringValue);
            }
            catch
            {
                return default(T);
            }
        }

        public static IReadOnlyList<string> GetAll(this IPrincipal principal, string claimType)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null) return new string[0];

            return claimsPrincipal
                .FindAll(claimType)
                .NullCheck()
                .Select(x => x.Value)
                .AsReadOnly();
        }
    }
}
