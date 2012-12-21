using System;
using System.Web.Mvc;
using System.Web.Routing;
using LowercaseRoutesMVC4;

namespace Momo.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRouteLowercase("home index", "", new {controller = "home", action = "index"});
            routes.MapRouteLowercase("home about", "about", new {controller = "home", action = "about"});

            routes.MapRouteLowercase("account", "account/{action}/{id}", new {controller = "account", action = "manage", id = UrlParameter.Optional});

            routes.MapRouteLowercase("shopping lists add", "add", new {controller = "shoppinglists", action = "add"});
            routes.MapRouteLowercase("shopping lists index", "{username}", new {controller = "shoppinglists", action = "index"});
            routes.MapRouteLowercase("shopping lists", "{username}/{shoppinglist}/{action}", new {controller = "shoppinglists", action = "show"});
        }
    }
}
