using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    public class ValidateShoppingListAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.IsRouteUsername(filterContext.HttpContext.User)) return;

            // At some point soon, figure out how to get the shopping list and verify access.
            // Currently only the owner has access

            filterContext.Result = new HttpNotFoundResult();
        }
    }
}
