using System.Web;
using System.Web.Optimization;

namespace tiket_reservation
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                 "~/Scripts/jquery.min.js",
                 "~/Scripts/bootstrap.min.js",
                 "~/Scripts/jquery.fullpage.extensions.min.js",
                 "~/Scripts/material.min.js",
                 "~/Scripts/ripples.min.js"
              ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/style.css",
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/bootstrap-material-design.min.css",
                "~/Content/css/jquery.fullpage.css",
                "~/Content/css/ripples.min.css",
                "~/Content/css/font-awesome/font-awesome.css"
            ));

        }
    }
}
