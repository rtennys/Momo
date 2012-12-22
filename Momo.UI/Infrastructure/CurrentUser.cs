using System;
using System.Web;

namespace Momo.UI
{
    public static class CurrentUser
    {
        public static bool IsRouteUsername()
        {
            var context = HttpContext.Current;
            return context.Request.RequestContext.RouteData.IsRouteUsername(context.User);
        }

        public static bool HasShoppingListAccess()
        {
            if (IsRouteUsername()) return true;

            // At some point soon, figure out how to get the shopping list and verify access.
            // Currently only the owner has access

            return false;
        }
    }
}
