using System.Linq;
using OnlineExam.Helpers;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
        public void Attention(string message)
        {
            if ((TempData.ContainsKey(Alerts.ATTENTION)))
                TempData.Add(Alerts.ATTENTION, message);
        }

        public void Success(string message)
        {
            if ((TempData.ContainsKey(Alerts.SUCCESS)))
                TempData.Add(Alerts.SUCCESS, message);
        }

        public void Information(string message)
        {
            if ((TempData.ContainsKey(Alerts.INFORMATION)))
                TempData.Add(Alerts.INFORMATION, message);
        }

        public void Error(string message)
        {
            if ((TempData.ContainsKey(Alerts.ERROR)))
                TempData.Add(Alerts.ERROR, message);
        }

        //After action is executed
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        //Before action executed
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

        }
    }
}