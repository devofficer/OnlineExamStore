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
        public ActionResult Teacher(int? page, string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            var dbContext = new ApplicationDbContext();
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
                      RegisterdDate = x.CreatedOn.ToString("yyyy-MM-dd")
                  }).OrderBy(x=>x.UserProfileId);
            
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(recordList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TeacherProfile(int id)
        {
            //var recordList = new List<TeacherViewModel>();
            var dbContext = new ApplicationDbContext();

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