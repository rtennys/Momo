using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;

namespace Momo.UI
{
    public static class MyMvcExtensions
    {
        public static bool IsRouteUsername(this RouteData routeData, IPrincipal principal)
        {
            return routeData.IsRouteUsername(principal.Identity);
        }

        public static bool IsRouteUsername(this RouteData routeData, IIdentity identity)
        {
            var routeUsername = routeData.Values["Username"] as string;
            var authUsername = identity.Name;

            return string.Equals(routeUsername, authUsername, StringComparison.OrdinalIgnoreCase);
        }


        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model = default(TModel))
        {
            return HtmlHelperFor(htmlHelper, model, null);
        }

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model, string htmlFieldPrefix)
        {
            var viewDataContainer = CreateViewDataContainer(htmlHelper.ViewData, model);

            var templateInfo = viewDataContainer.ViewData.TemplateInfo;

            if (!string.IsNullOrEmpty(htmlFieldPrefix))
                templateInfo.HtmlFieldPrefix = templateInfo.GetFullHtmlFieldName(htmlFieldPrefix);

            var viewContext = htmlHelper.ViewContext;
            var newViewContext = new ViewContext(viewContext.Controller.ControllerContext, viewContext.View, viewDataContainer.ViewData, viewContext.TempData, viewContext.Writer);

            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, htmlHelper.RouteCollection);
        }

        private static IViewDataContainer CreateViewDataContainer(ViewDataDictionary viewData, object model)
        {
            var newViewData = new ViewDataDictionary(viewData) {Model = model};

            newViewData.TemplateInfo = new TemplateInfo {HtmlFieldPrefix = newViewData.TemplateInfo.HtmlFieldPrefix};

            return new ViewDataContainer {ViewData = newViewData};
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
