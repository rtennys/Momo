using System;
using System.Web;
using System.Web.Mvc;
using Momo.Common;

namespace Momo.UI.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeActivityAttribute : AuthorizeAttribute
    {
        public AuthorizeActivityAttribute()
        {
            Order = 2;
        }

        private string _activity;

        private new string Roles { get; set; }
        private new string Users { get; set; }

        public string Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                Order = 1;
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;

            if (!user.Identity.IsAuthenticated) return false;

            var activity = _activity;

            if (string.IsNullOrEmpty(activity))
            {
                var routeData = httpContext.Request.RequestContext.RouteData;
                var controller = routeData.GetRequiredString("controller");
                var action = routeData.GetRequiredString("action");
                activity = "{0}/{1}".F(controller, action);
            }

            return user.IsInRole(activity);
        }
    }
}
