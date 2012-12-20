using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    public class RouteUsernameAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
                filterContext.RouteData.Values["username"] = filterContext.HttpContext.User.Identity.Name;
        }
    }
}
