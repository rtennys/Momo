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
            routes.MapRouteLowercase("Default", "{controller}/{action}/{id}", new {controller = "home", action = "index", id = UrlParameter.Optional});
        }
    }
}
