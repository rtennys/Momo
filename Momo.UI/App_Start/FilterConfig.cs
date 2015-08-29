using System;
using System.Web.Mvc;

namespace Momo.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyHandleErrorAttribute());
        }

        public class MyHandleErrorAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                var exception = filterContext.Exception;
                if (exception != null)
                    Logger.For(this).Error(exception.Message, exception);

                base.OnException(filterContext);
            }
        }
    }
}
