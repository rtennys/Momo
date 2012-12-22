using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Momo.UI.Controllers
{
    public class AppController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            AddUsername(requestContext);
        }

        private static void AddUsername(RequestContext requestContext)
        {
            if (!requestContext.HttpContext.Request.IsAuthenticated) return;

            var usernameKey = "Username";

            if (!requestContext.RouteData.Values.ContainsKey(usernameKey) || string.IsNullOrWhiteSpace((string)requestContext.RouteData.Values[usernameKey]))
                requestContext.RouteData.Values[usernameKey] = requestContext.HttpContext.User.Identity.Name;
        }
    }
}
