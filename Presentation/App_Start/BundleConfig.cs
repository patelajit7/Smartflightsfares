using System.Web;
using System.Web.Optimization;

namespace Presentation
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region Desktop JS

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/scripts/jquery.js",
                        "~/scripts/jquery-ui.js",
                        "~/scripts/slick.js",
                        "~/scripts/global.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/searchengine").Include(
                       "~/Scripts/js/searchengine.js"
                            ));

            bundles.Add(new ScriptBundle("~/bundles/listing").Include(
                    "~/scripts/js/listing.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/payment").Include(
                 "~/scripts/js/payment.js"
                 ));
            #endregion

            #region Desktop CSS
            bundles.Add(new StyleBundle("~/content/css").Include(
                       "~/content/bootstrap.min.css",
                      "~/content/jquery-ui.min.css",
                      "~/content/bt-icons.css",
                      "~/content/slick-theme.css",
                      "~/content/site.css"
                      ));
            bundles.Add(new StyleBundle("~/content/searchengine").Include(
                   "~/Content/searchengine.css"
                   ));
            bundles.Add(new StyleBundle("~/content/listing").Include(
                "~/Content/listing.css"
                    ));
            bundles.Add(new StyleBundle("~/content/payment").Include(
                    "~/content/payment.css"
                   ));
            #endregion

            #region Mobile JS
            bundles.Add(new ScriptBundle("~/bundles/m-js").Include(
                        "~/scripts/mobile/jquery.js",
                        "~/scripts/mobile/slick.js",
                        "~/scripts/mobile/global.js",
                        "~/scripts/mobile/html5.js"));

            bundles.Add(new ScriptBundle("~/bundles/m-searchengin-js").Include(
                        "~/scripts/mobile/searchengine.js"));

            bundles.Add(new ScriptBundle("~/bundles/m-listing-js").Include(
                        "~/scripts/mobile/listing.js"));

            bundles.Add(new ScriptBundle("~/bundles/m-payment-js").Include(
                        "~/scripts/mobile/payment.js"));
            #endregion

            #region Mibile CSS

            bundles.Add(new StyleBundle("~/content/m-css").Include(
                       "~/content/mobile/bootstrap.min.css",
                      "~/content/mobile/jquery-ui.min.css",
                      "~/content/mobile/bt-icons.css",
                      "~/content/mobile/slick-theme.css",
                      "~/content/mobile/site.css"
                      ));
            #endregion

            #region Widget Start            
            bundles.Add(new ScriptBundle("~/bundles/widget").Include(
                        "~/scripts/jquery.js",
                        "~/scripts/jquery-ui.js",
                        "~/scripts/slick.js",
                         "~/scripts/widget.js"
                         ));
            bundles.Add(new StyleBundle("~/content/widget").Include(
                      "~/content/bootstrap.min.css",
                      "~/content/jquery-ui.min.css",
                      "~/content/bt-icons.css",
                      "~/content/slick-theme.css",
                      "~/content/widget.css"
                ));            
            #endregion
        }
    }
}
