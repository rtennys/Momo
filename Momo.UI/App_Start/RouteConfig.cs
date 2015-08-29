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
            routes.MapRouteLowercase("home throw", "throw", new {controller = "home", action = "throw"});

            routes.MapRouteLowercase("account", "account/{action}/{id}", new {controller = "account", action = "manage", id = UrlParameter.Optional});
            routes.MapRouteLowercase("admin", "admin/{action}/{id}", new {controller = "admin", action = "index", id = UrlParameter.Optional});
            routes.MapRouteLowercase("ping", "ping/{action}/{id}", new {controller = "ping", action = "index", id = UrlParameter.Optional});

            routes.MapRouteLowercase("shopping lists add", "add", new {controller = "shoppinglists", action = "add"});
            routes.MapRouteLowercase("shopping lists index", "{username}", new {controller = "shoppinglists", action = "index"});
            routes.MapRouteLowercase("shopping lists edit item", "{username}/{shoppinglist}/{id}", new {controller = "shoppinglists", action = "edititem"}, new {id = @"^\d+$"});
            routes.MapRouteLowercase("shopping lists", "{username}/{shoppinglist}/{action}/{id}", new {controller = "shoppinglists", action = "show", id = UrlParameter.Optional});
        }
    }
}
