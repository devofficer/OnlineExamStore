using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Utils;
using OnlineExam.Models;
using System.Net.Mail;
using System.Net;
using PagedList;
using OnlineExam.Repositories;

namespace OnlineExam.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.IsError = false;
            ViewBag.Message = Convert.ToString(TempData["ConatctUsMessage"]);
            return View(new ContactUsViewModel());
        }
        [HttpPost]
        public ActionResult Contact(ContactUsViewModel contactUsViewModel)
        {
            if (ModelState.IsValid)
            {
                TempData["ConatctUsMessage"] = "Your query has sent successfully.";
                try
                {
                    string subject = contactUsViewModel.FirstName + " " + contactUsViewModel.LastName;
                    EmailService.ContactUs(contactUsViewModel.Email, contactUsViewModel.Description, subject);
                    //EmailService.SendMail(contactUsViewModel.Email, contactUsViewModel.Description, "Acada Store");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = AppConstants.ErrorMessageText;
                    ViewBag.IsError = true;
                    return View(contactUsViewModel);
                }

                return RedirectToAction("Contact");
            }
            return View(contactUsViewModel);
        }
        public ActionResult Course()
        {
            ViewBag.Message = "Your course page.";
            return View();
        }
        public ActionResult Teacher(int? page, string sortOrder, string lstClass, string lstSubject)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.lstClass = lstClass;
            ViewBag.lstSubject = lstSubject;
            bool logged = false;
            int currentUserProfileId = 0;
            ViewBag.CurrentUserId = 0;
            var dbContext = new ApplicationDbContext();
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();
               
               // if (User.IsInRole("Student"))
                //{
                    currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;
                ViewBag.CurrentUserId = currentUserProfileId;
                // }
                logged = true;
            }
            
          //  IList<TeacherViewModel> recordList = new List<TeacherViewModel>();
            //  var recordList = new List<TeacherViewModel>();
           var recordList = dbContext.UserProfiles.ToList()
                  .Select(x => new TeacherViewModel
                  {
                      UserProfileId = x.UserProfileId,
                      Image = x.Avatar,
                      Email = x.Email,
                      FullName = x.FirstName + " " + x.LastName,
                      Class = x.ClassTypes,
                      Subject = x.SubjectCategory,
                      RegisterdDate = x.CreatedOn.ToString("yyyy-MM-dd"),
                      Follow = IfFollowing(x.UserProfileId, currentUserProfileId, logged) == true?"Yes":"No"
                  });

            if (!string.IsNullOrEmpty(lstSubject))
            {
                recordList = recordList.Where(x => x.Subject != null);
                recordList = recordList.Where(x => x.Subject.Contains(lstSubject));
            }
            recordList = recordList.OrderBy(x => x.UserProfileId);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            var classCategories = CommonRepository.GetLookups("ClassCategory").DistinctBy(x=>x.Value);
           // var subjects = CommonRepository.GetLookups("SubjectCategory");
            ViewBag.classCategories = new SelectList(classCategories, "Value", "Text");
           // ViewBag.subjects = new SelectList(subjects, "Value", "Text");

            return View(recordList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public JsonResult GetSubjectsByClassCategory(string subjectCategory)
        {
            if (string.IsNullOrWhiteSpace(subjectCategory))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), subjectCategory);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubjectCategoryByClassTypes(string classType)
        {
            if (string.IsNullOrWhiteSpace(classType))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetSubjectCategoryByClassTypes(classType);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFollowsList(int id)
        {
            var dbContext = new ApplicationDbContext();
            
            string currentUserId = User.Identity.GetUserId();
            int currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;
            if (dbContext.Followers.Where(x=>x.UserProfileId == id && x.FollowersUserProfileId == currentUserProfileId).Count() == 0)
            {
                var info = new Followers();
                info.UserProfileId = id;
                info.FollowersUserProfileId = currentUserProfileId;
                info.StartDate = DateTime.Now;

                dbContext.Followers.Add(info);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Teacher");
        }

        public static bool IfFollowing(int userId1, int userId2, bool logged)
        {
            bool flag = false;

            if (logged == true)
            {
                var dbContext = new ApplicationDbContext();
                if (dbContext.Followers.Where(x => x.UserProfileId == userId1 && x.FollowersUserProfileId == userId2).Count() == 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public ActionResult TeacherProfile(int id)
        {
            bool logged = false;
            int currentUserProfileId = 0;
            var dbContext = new ApplicationDbContext();
            if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
            {
                string currentUserId = User.Identity.GetUserId();
                currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;
                logged = true;
            }

            var info = dbContext.UserProfiles.Find(id);

            ViewBag.Expertise = "";
            ViewBag.Qualifications = "";
            ViewBag.Offering = "";
            ViewBag.Lessons = "";

            if(dbContext.TeachersProfileExtended.Where(x=>x.UserProfileId == id).Count() > 0)
            {
                var techerInfo = dbContext.TeachersProfileExtended.FirstOrDefault(x => x.UserProfileId == id);
                ViewBag.Expertise = techerInfo.Expertise;
                ViewBag.Qualifications = techerInfo.Qualifications;
                ViewBag.Offering = techerInfo.Offering;
                ViewBag.Lessons = techerInfo.Lessons;
            }

            ViewBag.Following = IfFollowing(id, currentUserProfileId, logged) == true ? "Yes" : "No";

            return View(info);
        }

        public ActionResult Faq()
        {
            ViewBag.Message = "Your FAQs page.";
            return View();
        }
        public ActionResult Pricing()
        {
            ViewBag.Message = "Your Pricing page.";
            return View();
        }
        public ActionResult DownloadMaterial()
        {
            ViewBag.Message = "Your Download Material page.";
            return View();
        }
        public ActionResult Policy()
        {
            ViewBag.Message = "Your Policy page.";
            return View();
        }
        public ActionResult Terms()
        {
            ViewBag.Message = "Your Terms & Condition page.";
            return View();
        }
        public ActionResult OnlineCbt()
        {
            ViewBag.Message = "Your Online CBTs page.";
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}