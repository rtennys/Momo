using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Momo.UI.Controllers;
using WebMatrix.WebData;

namespace Momo.UI
{
    public class MvcApplication : HttpApplication
    {
        private static Logger _logger;

        protected void Application_Start()
        {
            Logger.Initialize();
            _logger = Logger.For<MvcApplication>();

            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            Application["version"] = version;
            Application["versionUrl"] = $"https://github.com/rtennys/Momo/commit/{version.Split('.').Last()}";
            Application["name"] = "mnmllist";

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            WebSecurity.InitializeDatabaseConnection("momo_conn", "UserProfile", "Id", "Username", true);
        }

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404) HandleNotFoundResponse();
        }

        private void HandleNotFoundResponse()
        {
            _logger.Warn("\"{0}\" not found", Request.RawUrl);

            Response.Clear();

            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "NotFound";

            var homeController = (IController)Ioc.Resolve<HomeController>();
            homeController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}
