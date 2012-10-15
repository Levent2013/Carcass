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
                        "~/Content/js/carcass-util.js",
                        "~/Content/js/bootstrap-datepicker.js",
                        "~/Content/js/bootstrap-timepicker.js",
                        "~/Content/js/bootstrap-fileinput.js",
                        "~/Content/tinymce/tiny_mce.js",
                        "~/Content/js/carcass-jquery.js",
                        "~/Content/js/carcass-mvc.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/js/jquery.unobtrusive*",
                        "~/Content/js/jquery.validate.js",
                        "~/Content/js/jquery.validate.additional-methods.js",
                        "~/Content/js/jquery.validate.unobtrusive.js",
                        "~/Content/js/carcass-validate.js")); // IMPORTANT: carcass-validate.js must be included after jquery.validate.js


            bundles.Add(new StyleBundle("~/Content/css/main")
                .Include("~/Content/css/bootstrap.css",
                    "~/Content/css/bootstrap-responsive.css",
                    "~/Content/css/bootstrap-fix.css",
                    "~/Content/css/bootstrap-datepicker.css",
                    "~/Content/css/bootstrap-timepicker.css",
                    "~/Content/css/bootstrap-fileinput.css",
                    "~/Content/css/carcass-mvc.css",
                    "~/Content/css/site.css"));
        }
    }
}