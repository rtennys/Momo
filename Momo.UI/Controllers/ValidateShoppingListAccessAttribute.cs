using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    public class ValidateShoppingListAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentUser.IsRouteUsername() || CurrentUser.HasShoppingListAccess()) return;

            filterContext.Result = new HttpNotFoundResult();
        }
    }
}
