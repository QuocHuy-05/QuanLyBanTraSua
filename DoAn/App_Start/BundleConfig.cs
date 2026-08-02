using System.Web;
using System.Web.Optimization;

namespace DACK_LTW_Nhom4
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/bootstrap-css").Include(
                "~/Content/bootstrap/css/bootstrap.min.css",
                "~/Content/Site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-js").Include(
                "~/Content/bootstrap/js/jquery-3.3.1.min.js",
                "~/Content/bootstrap/js/bootstrap.bundle.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"
            ));
        }
    }
}