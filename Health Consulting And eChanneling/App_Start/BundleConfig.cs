using System.Web;
using System.Web.Optimization;

namespace Health_Consulting_And_eChanneling
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));




            //Website Bundle Configularation
            bundles.Add(new StyleBundle("~/Content/healhConsultCSS").Include(
                      "~/Content/HealthConsultAllCSS/lib/bootstrap/css/bootstrap.min.css",
                      "~/Content/HealthConsultAllCSS/lib/font-awesome/css/font-awesome.min.css",
                      "~/Content/HealthConsultAllCSS/lib/animate/animate.min.css",
                      "~/Content/HealthConsultAllCSS/lib/ionicons/css/ionicons.min.css",
                      "~/Content/HealthConsultAllCSS/lib/owlcarousel/assets/owl.carousel.min.css",
                      "~/Content/HealthConsultAllCSS/lib/lightbox/css/lightbox.min.css",
                      "~/Content/HealthConsultAllCSS/css/style.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/healhConsultJs").Include(
                      "~/Content/HealthConsultAllCSS/lib/jquery/jquery.min.js",
                      "~/Content/HealthConsultAllCSS/lib/bootstrap/js/bootstrap.bundle.min.js",
                      "~/Content/HealthConsultAllCSS/lib/easing/easing.min.js",
                      "~/Content/HealthConsultAllCSS/lib/mobile-nav/mobile-nav.js",
                      "~/Content/HealthConsultAllCSS/lib/wow/wow.min.js",
                      "~/Content/HealthConsultAllCSS/lib/waypoints/waypoints.min.js",
                      "~/Content/HealthConsultAllCSS/lib/counterup/counterup.min.js",
                      "~/Content/HealthConsultAllCSS/lib/owlcarousel/owl.carousel.min.js",
                      "~/Content/HealthConsultAllCSS/lib/isotope/isotope.pkgd.min.js",
                      "~/Content/HealthConsultAllCSS/lib/lightbox/js/lightbox.min.js",
                      "~/Content/HealthConsultAllCSS/contactform/contactform.js",
                      "~/Content/HealthConsultAllCSS/js/main.js"
                      ));
        }
    }
}
