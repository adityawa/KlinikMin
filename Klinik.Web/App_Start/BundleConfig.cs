using System.Web;
using System.Web.Optimization;

namespace Klinik.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                          "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/Site.css"));

            //bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
            //         "~/Scripts/dropzone/dropzone.min.js"));

            //bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
            //         "~/Scripts/dropzone/css/basic.css",
            //         "~/Scripts/dropzone/css/dropzone.css"));

            //bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
            //        "~/Scripts/chosen/chosen.jquery.min.js",
            //        "~/Scripts/chosen/chosen.proto.min.js"
            //));

            //bundles.Add(new ScriptBundle("~/bundles/boostrapselect").Include(
            //        "~/Scripts/boostrap-select/bootstrap-select.min.js",
            //        "~/Scripts/boostrap-select/ajax-bootstrap-select.min.js"
            //));

            //bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
            //        "~/Scripts/DataTables/js/jquery.dataTables.js"
            //));

            //bundles.Add(new StyleBundle("~/Content/chosen").Include(
            //         "~/Scripts/chosen/chosen.min.css"));

            //bundles.Add(new StyleBundle("~/Content/boostrapselect").Include(
            //         "~/Scripts/boostrap-select/ajax-bootstrap-select.min.css",
            //         "~/Scripts/boostrap-select/bootstrap-select.min.css"
            //         ));

            //bundles.Add(new StyleBundle("~/Content/datatable").Include(
            //        "~/Scripts/DataTables/css/jquery.dataTables.css"
            //));

            //bundles.Add(new ScriptBundle("~/bundles/select2").Include(
            //        "~/Scripts/select2.min.js"
            //));

            //bundles.Add(new StyleBundle("~/Content/select2").Include(
            //         "~/Content/css/select2.min.css"));
        }
    }
}
