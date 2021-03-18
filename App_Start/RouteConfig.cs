using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineExam
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //routes.MapRoute("QuestionBankIndex", "Question/Page/{page}", new { controller = "Question", action = "Index", page = UrlParameter.Optional });
            routes.MapRoute("QuestionPaging","{page}",
                            new { controller = "Question", action = "Index" },
                            new { page = @"\d+" },
                            new[] { "OnlineExam.Controllers" });
            //http://localhost:44300/Account/Register?register=Demo
            routes.MapRoute("Register", "Account/Register/{register}", new { controller = "Account", action = "Register" });
            routes.MapRoute("CompanyUpdate", "Company/Update/{id}/{status}", new { controller = "Company", action = "Update" });
        }
    }
}
