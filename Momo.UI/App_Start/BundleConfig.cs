using System;
using System.Web.Optimization;

namespace Momo.UI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterCss(bundles);
            RegisterJavascript(bundles);
        }

        private static void RegisterCss(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/jquery.mobile-{version}.css",
                "~/Content/toastr.css",
                "~/Content/site.css"));
        }

        private static void RegisterJavascript(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/unminimized").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.mobile-{version}.js",
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.mapping-latest.js",
                "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/minimized").Include(
                "~/Scripts/Site.js"));
        }
    }
}
