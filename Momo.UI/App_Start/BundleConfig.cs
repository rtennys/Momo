using System;
using System.Web.Optimization;

namespace Momo.UI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/common/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.mobile-{version}.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/Site.js"));

            bundles.Add(new StyleBundle("~/bundles/common/css").Include(
                "~/Content/jquery.mobile-{version}.css",
                "~/Content/site.css"));
        }
    }
}
