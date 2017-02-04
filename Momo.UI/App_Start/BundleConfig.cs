using System;
using System.Web.Optimization;

namespace Momo.UI
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
#if DEBUG
            //BundleTable.EnableOptimizations = false;
#endif

            bundles.Add(GetScriptBundle("~/js").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/toastr.js",
                "~/Scripts/Site.js",
                "~/Scripts/Site.*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/toastr.css",
                "~/Content/site.css"));
        }

        private static ScriptBundle GetScriptBundle(string virtualPath)
        {
            return new ScriptBundle(virtualPath) { ConcatenationToken = ";" + Environment.NewLine };
        }
    }
}
