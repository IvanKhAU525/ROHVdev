using System.Web;
using System.Web.Optimization;

namespace ROHV
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery/dist/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/libs/jquery-validation/dist/jquery.validate.min.js",
                        "~/Scripts/libs/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/libs/bootstrap/dist/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                    "~/Scripts/js/Login.js",
                    "~/Scripts/js/Utils.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Scripts/libs/bootstrap/dist/css/bootstrap-theme.min.css",
                    "~/Scripts/libs/bootstrap/dist/css/bootstrap.min.css",
                    "~/Scripts/libs/dropzone/dist/basic.css",
                    "~/Scripts/libs/dropzone/dist/dropzone.css",
                      "~/Content/site.css"));
        }
    }
}
