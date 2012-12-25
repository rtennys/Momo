using System;
using System.Linq;
using System.Web;
using Momo.Common;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;

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
            var context = HttpContext.Current;
            var routeData = context.Request.RequestContext.RouteData;

            if (routeData.IsRouteUsername(context.User)) return true;

            var shoppingListAccessKey = "_hasShoppingListAccess";
            if (!routeData.Values.ContainsKey(shoppingListAccessKey))
            {
                var authUsername = context.User.Identity.Name;
                var listOwner = routeData.Values["username"] as string;
                var listName = routeData.Values["shoppinglist"] as string;

                routeData.Values[shoppingListAccessKey] = Ioc.Resolve<IRepository>()
                    .Find<ShoppingList>()
                    .SelectMany(x => x.SharedWith, (shoppingList, sharedUser) => new {shoppingList, sharedUser})
                    .Where(x => x.shoppingList.UserProfile.Username == listOwner)
                    .Where(x => x.shoppingList.Name == listName)
                    .Any(x => x.sharedUser.Username == authUsername);
            }

            return (bool)routeData.Values[shoppingListAccessKey];
        }
    }
}
