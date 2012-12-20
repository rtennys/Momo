using System;
using System.Web.Mvc;
using Momo.UI.Controllers;

namespace Momo.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RouteUsernameAttribute());
        }
    }
}
