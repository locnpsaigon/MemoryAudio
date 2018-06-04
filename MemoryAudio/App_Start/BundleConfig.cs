using System.Web;
using System.Web.Optimization;

namespace MemoryAudio
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/scripts/js/responsee.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts-admin").Include(
                       "~/Content/bower_components/bootstrap/dist/js/bootstrap.min.js",
                       "~/Content/bower_components/fastclick/lib/fastclick.js",
                       "~/Content/dist/js/adminlte.min.js",
                       "~/Content/bower_components/jquery-sparkline/dist/jquery.sparkline.min.js",
                       "~/Content/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                       "~/Content/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                       "~/Content/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                       "~/Content/bower_components/Chart.js/Chart.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css-web").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/css/components.css",
                      "~/Content/css/icons.css",
                      "~/Content/css/responsee.css",
                      "~/Content/css/template-style.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css-admin").Include(
                      "~/Content/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/Content/bower_components/Ionicons/css/ionicons.min.css",
                      "~/Content/bower_components/jvectormap/jquery-jvectormap.css",
                      "~/Content/dist/css/AdminLTE.css",
                      "~/Content/dist/css/skins/_all-skins.min.css"));
        }
    }
}
