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
        public ActionResult Teacher()
        {
            // ViewBag.Message = "Your Teacher page.";
          //  var info = db.UserProfiles.Where(x => x.ApplicationUser.UserType== "Teacher" && x.ApplicationUser.Status=="Active").ToList();
            return View();
        }

        [Route("/Home/TeachersList")]
        private ActionResult TeachersList()
        {
            //var recordList = new List<TeacherViewModel>();
            var dbContext = new ApplicationDbContext();

              var recordList = dbContext.UserProfiles.Where(x => x.ApplicationUser.Status == "Active" && x.ApplicationUser.UserType == "Teacher")
                    .Select(x => new TeacherViewModel
                    {
                        UserProfileId = x.UserProfileId,
                        Image = x.Avatar,
                        Email = x.Email,
                        FullName = x.FullName,
                        Class = x.ClassTypes,
                        Subject = x.SubjectCategory,
                        RegisterdDate = x.CreatedOn.ToString("yyyy-MM-dd")
                    }).ToList();

            return Json(recordList, JsonRequestBehavior.AllowGet);
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