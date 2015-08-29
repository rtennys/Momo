using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Momo.Common
{
    public static class ThreadHelper
    {
        public static IPrincipal CurrentPrincipal()
        {
            var context = HttpContext.Current;
            return context != null ? context.User : Thread.CurrentPrincipal;
        }

        public static bool IsAuthenticated()
        {
            return CurrentPrincipal().Identity.IsAuthenticated;
        }

        public static string GetClaim(string claimType)
        {
            return CurrentPrincipal().Get(claimType);
        }

        public static T GetClaim<T>(string claimType)
        {
            return CurrentPrincipal().Get<T>(claimType);
        }

        public static IReadOnlyList<string> GetAllClaims(string claimType)
        {
            return CurrentPrincipal().GetAll(claimType);
        }

        public static void RunAs(IPrincipal principal, Action action)
        {
            RunAs(principal, () =>
            {
                action();
                return 0;
            });
        }

        public static T RunAs<T>(IPrincipal principal, Func<T> func)
        {
            var originalThread = Thread.CurrentPrincipal;
            var originalContext = HttpContext.Current != null ? HttpContext.Current.User : null;

            try
            {
                principal = principal ?? new GenericPrincipal(new GenericIdentity(""), new string[0]);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null) HttpContext.Current.User = principal;

                return func();
            }
            finally
            {
                Thread.CurrentPrincipal = originalThread;
                if (HttpContext.Current != null) HttpContext.Current.User = originalContext;
            }
        }
    }
}
