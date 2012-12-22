using System;
using System.Security.Principal;
using System.Web.Routing;

namespace Momo.UI
{
    public static class MyMvcExtensions
    {
        public static bool IsRouteUsername(this RouteData routeData, IPrincipal principal)
        {
            return routeData.IsRouteUsername(principal.Identity);
        }

        public static bool IsRouteUsername(this RouteData routeData, IIdentity identity)
        {
            var routeUsername = routeData.Values["Username"] as string;
            var authUsername = identity.Name;

            return string.Equals(routeUsername, authUsername, StringComparison.OrdinalIgnoreCase);
        }
    }
}
