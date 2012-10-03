using System.Web;
using System.Web.Optimization;

namespace Carcass
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/startup").Include(
                        "~/Content/js/modernizr-*",
                        "~/Content/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Content/js/bootstrap.js",
                        "~/Content/js/carcass.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/js/jquery.unobtrusive*",
                        "~/Content/js/jquery.validate*"));


            bundles.Add(new StyleBundle("~/Content/css/main")
                .Include("~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-responsive.css",
                "~/Content/css/bootstrap-fix.css",
                "~/Content/css/site.css"));
        }
    }
}