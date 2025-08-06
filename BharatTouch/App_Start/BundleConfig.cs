using System.Web;
using System.Web.Optimization;
using static QRCoder.PayloadGenerator;
using System.Web.UI;
using iTextSharp.tool.xml.css;
using BharatTouch.Helper;

namespace BharatTouch
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/homeCss").Include(
                     //"~/Content/vendor/bootstrap/css/bootstrap.css", /*Vendor CSS*/
                                                                     //"~/Content/vendor/font-awesome/css/font-awesome.css",
                     "~/Content/vendor/magnific-popup/magnific-popup.css",
                     "~/Content/vendor/bootstrap-datepicker/css/datepicker3.css",
                     //"~/Content/vendor/jquery-ui/css/ui-lightness/jquery-ui-1.10.4.custom.css",//Specific Page Vendor CSS*/
                     "~/Content/vendor/select2/select2.css",
                     "~/Content/vendor/bootstrap-multiselect/bootstrap-multiselect.css",
                     //"~/Content/vendor/morris/morris.css",
                     //"~/Content/stylesheets/theme.css",/*Theme CSS*/
                     //"~/Content/stylesheets/skins/default.css",/*Skin CSS */
                     //"~/Content/stylesheets/theme-custom.css", /*Theme Custom CSS*/
                     "~/Content/stylesheets/datatable1.10.9.css", /*Server side datatable CSS*/
                    "~/Content/javascripts/custom/pnotify/pnotify.css", /*notification*/
                    "~/Content/vendor/cropper/cropper.css"
                   
                     ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/vendor/bootstrap/css/bootstrap.css", /*Vendor CSS*/
                                                                      //"~/Content/vendor/font-awesome/css/font-awesome.css",
                      "~/Content/vendor/magnific-popup/magnific-popup.css",
                      "~/Content/vendor/bootstrap-datepicker/css/datepicker3.css",
                      //"~/Content/vendor/jquery-ui/css/ui-lightness/jquery-ui-1.10.4.custom.css",//Specific Page Vendor CSS*/
                      "~/Content/vendor/select2/select2.css",
                      "~/Content/vendor/bootstrap-multiselect/bootstrap-multiselect.css",
                      "~/Contentskydashalam.css",
                      //"~/Content/stylesheets/theme.css",/*Theme CSS*/
                      //"~/Content/stylesheets/skins/default.css",/*Skin CSS */
                      //"~/Content/stylesheets/theme-custom.css", /*Theme Custom CSS*/
                      "~/Content/stylesheets/datatable1.10.9.css", /*Server side datatable CSS*/
                      "~/Content/javascripts/custom/pnotify/pnotify.css", /*notification*/
                      "~/Skydash/vendors/feather/feather.css",/*skydash start here*/
                      "~/Skydash/vendors/ti-icons/css/themify-icons.css",
                      "~/Skydash/vendors/css/vendor.bundle.base.css",
                      "~/Skydash/vendors/ti-icons/css/themify-icons.css",
                      "~/Skydash/css/vertical-layout-light/style.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/reportCss").Include(
                     "~/Content/vendor/bootstrap/css/bootstrap.css", /*Vendor CSS*/
                     "~/Content/vendor/font-awesome/css/font-awesome.css",
                      "~/Content/stylesheets/theme.css",/*Theme CSS*/
                      "~/Content/stylesheets/skins/default.css",/*Skin CSS */
                      "~/Content/stylesheets/theme-custom.css" /*Theme Custom CSS*/
                     ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     "~/Content/vendor/modernizr/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/VendorJs").Include(
                    "~/Content/vendor/jquery/jquery.js",
                    "~/Content/vendor/jquery-browser-mobile/jquery.browser.mobile.js",
                    "~/Content/vendor/bootstrap/js/bootstrap.js",
                    "~/Content/vendor/nanoscroller/nanoscroller.js",
                    "~/Content/vendor/bootstrap-datepicker/js/bootstrap-datepicker.js",
                    "~/Content/vendor/magnific-popup/magnific-popup.js",
                    "~/Content/vendor/jquery-placeholder/jquery.placeholder.js",

                    //"~/Content/vendor/pdf-js/pdf.min.js"
                    //"~/Content/vendor/pdf-js/pdf.worker.min.js"
                    "~/Content/vendor/cropper/cropper.js",
                    "~/Content/vendor/canvas-to-blob/canvas-to-blob.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/SpecificPageJS").Include(
                     "~/Content/vendor/jquery-ui/js/jquery-ui-1.10.4.custom.js",
                     "~/Content/vendor/jquery-ui-touch-punch/jquery.ui.touch-punch.js",
                     "~/Content/vendor/jquery-appear/jquery.appear.js",
                     "~/Content/vendor/select2/select2.js",
                     "~/Content/vendor/bootstrap-multiselect/bootstrap-multiselect.js",
                     "~/Content/vendor/jquery-maskedinput/jquery.maskedinput.js",
                    ////// //"~/Content/vendor/jquery-easypiechart/jquery.easypiechart.js",
                    ////// //"~/Content/vendor/flot/jquery.flot.js",
                    ////// //"~/Content/vendor/flot-tooltip/jquery.flot.tooltip.js",
                    ////// //"~/Content/vendor/flot/jquery.flot.pie.js",
                    //////// "~/Content/vendor/flot/jquery.flot.categories.js",
                    //////// "~/Content/vendor/flot/jquery.flot.resize.js",
                    //////// "~/Content/vendor/jquery-sparkline/jquery.sparkline.js",
                    ////// //"~/Content/vendor/raphael/raphael.js",
                    ////// //"~/Content/vendor/morris/morris.js",
                    ////// //"~/Content/vendor/gauge/gauge.js",
                    ////// //"~/Content/vendor/snap-svg/snap.svg.js",
                    //////// "~/Content/vendor/liquid-meter/liquid.meter.js",
                    ////// //"~/Content/vendor/jqvmap/jquery.vmap.js",
                    ////// //"~/Content/vendor/jqvmap/data/jquery.vmap.sampledata.js",
                    ////// //"~/Content/jqvmap/jqvmap/maps/jquery.vmap.world.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.africa.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.asia.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.australia.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.europe.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.north-america.js",
                    ////// //"~/Content/vendor/jqvmap/maps/continents/jquery.vmap.australia.js",
                     "~/Content/javascripts/theme.js",/*Theme*/
                     "~/Content/javascripts/theme.custom.js",/*Theme costom*/
                     "~/Content/javascripts/theme.init.js",/*Theme Initialization Files*/
                     //////"~/Content/javascripts/dashboard/examples.dashboard.js",/*Exaple*/
                     "~/Content/javascripts/custom/pnotify/pnotify.js", /*notification*/
                     "~/Content/javascripts/custom/pnotify/PNotifyStyleMaterial.js", /*notification*/
                     "~/Content/javascripts/custom/pnotify/PNotifyButtons.js", /*notification*/
                     "~/Content/javascripts/custom/pnotify/PNotifyConfirm.js", /*notification*/
                     "~/Content/javascripts/custom/pnotify/PNotifyMobile.js", /*notification*/
                     "~/Content/javascripts/custom/jquery.validate.min.js",/*to prevent postback on submit button*/
                     "~/Content/javascripts/custom/Shared.js"/*common functions*/
                     //"~/Content/javascripts/jquery.userTimeout.js"/*common functions*/                     
                 ));

            bundles.Add(new StyleBundle("~/bundles/formLayoutCss").Include(
                "~/FormAssets/vendor/googleapis/fonts.css",
                //Vendor CSS Files
                "~/FormAssets/vendor/bootstrap/css/bootstrap.min.css",
                //"~/HomeAssets/vendor/bootstrap-icons/bootstrap-icons.css", // not working with minification bundle
                //"~/FormAssets/vendor/icofont/icofont.min.css", // not working with minification bundle
                //"~/FormAssets/vendor/boxicons/css/boxicons.min.css", // not working with minification bundle
                "~/FormAssets/vendor/venobox/venobox.css",
                "~/FormAssets/vendor/owl.carousel/passets/owl.carousel.min.css",
                "~/FormAssets/vendor/aos/aos.css",
                "~/HomeAssets/vendor/swiper/swiper-bundle.min.css",
                //Template Main CSS File
                "~/FormAssets/css/style.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/formLayoutJs").Include(
                //Vendor JS Files
                //"~/FormAssets/vendor/jquery/jquery.min.js",
                //"~/HomeAssets/vendor/bootstrap/js/bootstrap.bundle.js", //not working in bundle
                "~/FormAssets/vendor/jquery.easing/jquery.easing.min.js",
                "~/FormAssets/vendor/waypoints/jquery.waypoints.min.js",
                "~/FormAssets/vendor/counterup/counterup.min.js",
                "~/FormAssets/vendor/isotope-layout/isotope.pkgd.min.js",
                "~/FormAssets/vendor/venobox/venobox.min.js",
                "~/FormAssets/vendor/owl.carousel/owl.carousel.min.js",
                "~/FormAssets/vendor/typed.js/typed.min.js",
                "~/FormAssets/vendor/aos/aos.js",
                "~/HomeAssets/vendor/swiper/swiper-bundle.min.js",
                //Template Main JS File
                "~/FormAssets/js/main.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/homeLayoutCSS").Include(
                    "~/HomeAssets/vendor/googleapis/fonts.css",
                    //Vendor CSS Files
                    "~/HomeAssets/vendor/bootstrap/css/bootstrap.min.css",
                    //"~/HomeAssets/vendor/bootstrap-icons/bootstrap-icons.css", // not working with minification bundle
                    //"~/HomeAssets/vendor/aos/aos.css", // not working with datatable
                    //"~/HomeAssets/vendor/remixicon/remixicon.css", // not working with minification bundle
                    "~/HomeAssets/vendor/swiper/swiper-bundle.min.css",
                    "~/HomeAssets/vendor/glightbox/css/glightbox.min.css",
                    //Template Main CSS File
                    "~/HomeAssets/css/style.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/homeLayoutJS").Include(
                //Vendor JS Files
                //"~/HomeAssets/vendor/bootstrap/js/bootstrap.bundle.js", //not working in bundle
                "~/HomeAssets/vendor/aos/aos.js",
                "~/HomeAssets/vendor/swiper/swiper-bundle.min.js",
                "~/HomeAssets/vendor/purecounter/purecounter.js",
                "~/HomeAssets/vendor/isotope-layout/isotope.pkgd.min.js",
                "~/HomeAssets/vendor/glightbox/js/glightbox.min.js"

                //Template Main JS File
                //"~/HomeAssets/js/main.js"  // not working with minification bundle
            ));

            bundles.Add(new ScriptBundle("~/bundles/pageScriptJS").Include(
                "~/Content/page-scripts/Users/Authentication.js",
                "~/Content/page-scripts/Profile/Index.js",
                //"~/Content/page-scripts/Users/Create.js"
                "~/Content/page-scripts/Users/Index.js",
                "~/Content/page-scripts/CardTheme/Index.js",
                "~/Content/page-scripts/Users/FeatureNotification.js",
                "~/Content/page-scripts/AviPdf/Users/PdfUsers.js"
                ));

            //faiz added
            var validateJs = new ScriptBundle("~/bundles/validateJs").Include(
              //"~/admin_theme/js/jquery-3.6.0.min.js",
              "~/admin_theme/js/custom/jquery.validate.min.js",
              "~/admin_theme/js/jquery.dataTables.min.js",
              "~/admin_theme/js/flatpickrv4.6.13.js",
              "~/admin_theme/js/custom/Pnotify/pnotify.js",
              "~/admin_theme/js/custom/Pnotify/PNotifyStyleMaterial.js",
              "~/admin_theme/js/custom/Pnotify/PNotifyButtons.js",
              "~/admin_theme/js/custom/Pnotify/PNotifyConfirm.js",
              "~/admin_theme/js/custom/Pnotify/PNotifyMobile.js",
               "~/admin_theme/js/select2/select2.js",
               "~/admin_theme/js/moment/moment-with-locales.js",
               "~/admin_theme/js/moment/moment.js",
              "~/admin_theme/js/custom/Shared.js",
              "~/admin_theme/js/custom/sweetalert/sweetalert.min.js"
          );

            //// Clear default transforms and add custom minification
            validateJs.Transforms.Clear();
            validateJs.Transforms.Add(new CustomJsMinify());

            bundles.Add(validateJs);
            //faiz added
            var adminThemeBundle = new ScriptBundle("~/bundles/admin_theme").Include(
                 //"~/admin_theme/vendor/apexcharts/apexcharts.min.js",
                 "~/admin_theme/vendor/bootstrap/js/bootstrap.bundle.min.js",
                 //"~/admin_theme/vendor/chart.js/chart.umd.js",
                 //"~/admin_theme/vendor/echarts/echarts.min.js",
                 //"~/admin_theme/vendor/quill/quill.js",
                 //"~/admin_theme/vendor/simple-datatables/simple-datatables.js",

                 "~/admin_theme/vendor/tinymce/tinymce.min.js",
                 //"~/admin_theme/vendor/php-email-form/validate.js",
                 "~/admin_theme/js/main.js"
             // "~/admin_theme/js/custom/jquery.validate.min.js",/*to prevent postback on submit button*/
             // "~/admin_theme/js/custom/Shared.js"/*common functions*/
             );

            //// Clear default transforms and add custom minification
            adminThemeBundle.Transforms.Clear();
            adminThemeBundle.Transforms.Add(new CustomJsMinify());

            bundles.Add(adminThemeBundle);

            //faiz added
            // New bundle for admin theme CSS files
            bundles.Add(new StyleBundle("~/Content/admin_theme").Include(
                      "~/admin_theme/vendor/bootstrap/css/bootstrap.min.css",
                      "~/admin_theme/vendor/bootstrap-icons/bootstrap-icons.css",
                      "~/admin_theme/vendor/boxicons/css/boxicons.min.css",
                      "~/admin_theme/vendor/quill/quill.snow.css",
                      "~/admin_theme/vendor/quill/quill.bubble.css",
                      "~/admin_theme/vendor/remixicon/remixicon.css",
                      "~/admin_theme/vendor/simple-datatables/style.css",
                      "~/admin_theme/js/select2/select2.css",
                      "~/admin_theme/css/style.css",
                      "~/admin_theme/css/datatable1.10.9.css",
                      "~/admin_theme/js/custom/Pnotify/pnotify.css",
                      "~/admin_theme/css/flatpickr.min.css",
                      "~/admin_theme/js/custom/sweetalert/sweetalert.css"
                      ));

            //bundles.Add(new StyleBundle("~/DarkMode/css").Include(
            //    "~/FormAssets/vendor/icofont/icofont.min.css",
            //    "~/FormAssets/vendor/boxicons/css/boxicons.min.css",

            //    //cdn paths
            //    //"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.1/css/all.min.css",
            //    //"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.11.3/font/bootstrap-icons.min.css",

            //    "~/Content/javascripts/custom/pnotify/pnotify.css",
            //    //"~/Content/vendor/modernizr/modernizr.js",                    just for remember its used on page in this fomat             
            //    "~/FormAssets/vendor/bootstrap/css/bootstrap.min.css",
            //    "~/HomeAssets/vendor/swiper/swiper-bundle.min.css",
            //    "~/FormAssets/css/style.css",
            //    "/HomeAssets/css/testimonial.css",
            //    "~/MultiThemes/DarkMode/Css/CustomTheme.css"
            //));

            BundleTable.EnableOptimizations = false;

        }
    }
}
