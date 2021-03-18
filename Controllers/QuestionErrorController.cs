using System.Linq;
using OnlineExam.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Models;
using System.Collections.Generic;
using OnlineExam.Utils;
using OnlineExam.Infrastructure;

namespace OnlineExam.Controllers
{
    [AuthorizeUser(Roles = "Admin, StaffAdmin")]
    public class QuestionErrorController : Controller
    {
        public ActionResult Index()
        {
            return View(new List<QuestionError>());
        }
        [HttpGet]
        public JsonResult GetAll()
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                return Json(contextObj.Database
                    .SqlQuery<QuestionErrorViewModel>("GetQuestionErrors_SP")
                    .ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public string UpdateAction(int Id, String ActionTaken)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var dd = contextObj.QuestionErrors.Where(x => x.Id == Id).FirstOrDefault();
                dd.ActionTaken = ActionTaken;
                dd.ModifiedOn = DateTime.Now;
                dd.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                contextObj.SaveChanges();
                return AppConstants.SuccessMessageText;
            }
        }
    }
}