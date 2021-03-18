using System.Web;
using System.Web.Optimization;

namespace OnlineExam
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.FileSetOrderList.Clear();

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/bootstrap-datepicker.css",
                     "~/fonts/font-awesome.min.css",
                     "~/Scripts/clean-zone-admin-template/css/style.css",
                //"~/Content/toastr.css",
                //"~/Scripts/clean-zone-admin-template/js/bootstrap.datetimepicker/css/bootstrap-datetimepicker.min.css",
                     "~/Scripts/clean-zone-admin-template/js/bootstrap.switch/bootstrap-switch.css",
                     "~/Content/Choosen/chosen.css",
                     "~/Scripts/clean-zone-admin-template/js/jquery.nanoscroller/nanoscroller.css",
                     "~/Content/angular-block-ui.css"
                //"~/Scripts/clean-zone-admin-template/js/jquery.select2/select2.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.1.min.js",
                        "~/Scripts/jquery.form.js",
                        "~/Scripts/clean-zone-admin-template/js/jquery.ui/jquery-ui.js",
                        "~/Scripts/jquery.ext.controls.js", // for custom functions
                        "~/Scripts/ajaxHelper.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/respond.min.js",
                //"~/Content/toaster/js/toastr.min.js",
                        "~/Scripts/Choosen/chosen.jquery.js"
                //"~/Scripts/app/alerts.js"
                        ));
            //.Include("~/Scripts/angular/angular.js")

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
           "~/Scripts/angular/angular.min.js",
           "~/Scripts/angular/angular-route.min.js",
                //"~/Scripts/angular-sanitize.min.js",
                //"~/Scripts/angular-ui.min.js",
                //"~/Scripts/angular-ui/ui-bootstrap.min.js",
                //"~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                //"~/Scripts/angular-ui.min.js",
           "~/Scripts/angular/angular-block-ui.js"
        ));

            bundles.Add(new ScriptBundle("~/bundles/angularTimer").Include(
          "~/Scripts/angular-timer/angular-timer-all.min.js"
       ));

            bundles.Add(new ScriptBundle("~/bundles/school").Include(
         "~/Content/angular/school/Module.js",
         "~/Content/angular/school/Service.js",
         "~/Content/angular/school/Controller.js"
      ));

            bundles.Add(new ScriptBundle("~/bundles/smartQuiz").Include(
      "~/Content/angular/smart-quiz/quiz-controller.js",
                "~/Content/angular/smart-quiz/quiz-factory.js"
   ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/clean-zone-admin-template/js/bootstrap/bootstrap.min.js",
                        "~/Scripts/clean-zone-admin-template/js/behaviour/general.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/clean-zone-admin-template/js/jquery.nanoscroller/jquery.nanoscroller.js",
                        "~/Scripts/chosen.jquery.min.js",
                        "~/Scripts/clean-zone-admin-template/js/bootstrap.switch/bootstrap-switch.min.js"
                      ));



            //bundles.Add(new StyleBundle("~/Content/bootstrap-switch-css").Include(              
            //   "~/Scripts/clean-zone-admin-template/js/bootstrap.switch/bootstrap-switch.css"));

            //bundles.Add(new ScriptBundle("~/Script/bootstrap-switch-js").Include(
            //   "~/Scripts/clean-zone-admin-template/js/bootstrap.switch/bootstrap-switch.min.js"));
        }
    }
}
