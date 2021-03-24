using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    public class LessonsController : Controller
    {
        // GET: Lessons
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateNote()
        {
            var classCategory = CommonRepository.GetLookups("ClassCategory").DistinctBy(x => x.Value);
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult CreateNote(LessonNotes model)
        {
            if (!string.IsNullOrEmpty(model.ClassCategory) && !string.IsNullOrEmpty(model.Subject))
            {
                var dbContext = new ApplicationDbContext();
                string currentUserId = User.Identity.GetUserId();
                int currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;

                model.ProfileId = currentUserProfileId;
                model.CreatedDate = DateTime.Now;
                dbContext.Lessons.Add(model);
                dbContext.SaveChanges();
                return View().WithSuccess(AppConstants.SuccessMessageText);
            }
            else
            {
                var classCategory = CommonRepository.GetLookups("ClassCategory").DistinctBy(x => x.Value);
                ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");
                return RedirectToAction("CreateNote").WithError("Class category and subject must be select.");
            }
           
        }

        [HttpGet]
        public JsonResult GetSubjectCategoryByClass(string classcategory)
        {
            if (string.IsNullOrWhiteSpace(classcategory))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), classcategory);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTopicBySubjectCategory(string subjectcategory)
        {
            if (string.IsNullOrWhiteSpace(subjectcategory))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.Topic.ToString(), subjectcategory);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
    }
}