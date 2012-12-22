using System;
using System.Security.Principal;
using System.Web.Mvc;
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

        public static bool IsRouteUsername(this HtmlHelper helper)
        {
            return helper.ViewContext.RouteData.IsRouteUsername(helper.ViewContext.HttpContext.User);
        }

        public static bool HasShoppingListAccess(this HtmlHelper helper)
        {
            if (helper.IsRouteUsername()) return true;

            // At some point soon, figure out how to get the shopping list and verify access.
            // Currently only the owner has access

            return false;
        }
    }
}
