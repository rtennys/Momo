using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    public class ValidateRouteUsername : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentUser.IsRouteUsername()) return;

            filterContext.Result = new HttpNotFoundResult();
        }
    }
}
