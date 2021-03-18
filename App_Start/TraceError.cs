using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam
{
    public class TraceError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;


            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            var model = new HandleErrorInfo(filterContext.Exception, controller, action);

            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(model),
            };

            base.OnException(filterContext);
            
            Logger.WriteLogFile("Controller: "+ controller + ", Action: " + action, ex.Message, ex.StackTrace);

            string errorMsg = "Controller: " + controller + ", Action: " + action + " Message:  " + ex.Message + " StackTrace: " + ex.StackTrace;
            

            //Logger.WriteLogFile(eventName, ex.Message, ex.StackTrace);
            //filterContext.HttpContext.Response.Redirect("/Home/Error");
            //RedirectToAction("Error", "Home");
        }
    }
}