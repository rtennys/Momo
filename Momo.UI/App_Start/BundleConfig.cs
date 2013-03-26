using System;
using System.Web.Optimization;

namespace Momo.UI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));


            bundles.Add(new Bundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.mapping-latest.js",
                "~/Scripts/toastr.js",
                "~/Scripts/Site.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/toastr.css",
                "~/Content/site.css"));
        }
    }
}
