using System.Data.Entity;
using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Repositories;
using OnlineExam.Utils;

namespace OnlineExam.Controllers
{
    [Authorize(Roles = "Admin, StaffAdmin")]
    public class SystemSettingsController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        #region ---StepFirst
        [HttpGet]
        public ActionResult _Class()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult _Class(string text, int? id)
        {
            AddEditLookup(text, text, AppConstants.LookupType.ClassType.ToString(), id, string.Empty);
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult _Category()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult _Category(string category, int? id)
        {
            AddEditLookup(category, category, AppConstants.LookupType.ExamType.ToString(), id, string.Empty);
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult _Subject()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult _Subject(string subject, int? id)
        {
            AddEditLookup(subject, subject, AppConstants.LookupType.Subject.ToString(), id, string.Empty);
            return PartialView();
        }
        #endregion ---StepFirst

        [HttpGet]
        public PartialViewResult _ClassCategory()
        {
            ViewBag.drpClass = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ClassType.ToString());
            ViewBag.drpCategory = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ExamType.ToString());
            return PartialView();
        }
        [HttpPost]
        public ActionResult _ClassCategory(string iClass, string iCategory, string strCategory, int? id)
        {
            //AddLookUP(iClass, iCategory, strCategory, ModuleCode, id);
            AddEditLookup(strCategory, iCategory, AppConstants.LookupType.ClassCategory.ToString(), id, iClass);
            ViewBag.drpClass = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ClassType.ToString());
            ViewBag.drpCategory = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ExamType.ToString());
            return PartialView();
        }
        [HttpGet]
        public PartialViewResult _SubjectCategory()
        {
            ViewBag.drpSubject = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.Subject.ToString());
            ViewBag.drpCategory1 = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ExamType.ToString());
            return PartialView();
        }
        [HttpPost]
        public ActionResult _SubjectCategory(string iCategory, string iSubject, string strSubject, int? id)
        {
            //AddLookUP(iClass, iCategory, strCategory, ModuleCode, id);
            AddEditLookup(strSubject, iSubject, AppConstants.LookupType.SubjectCategory.ToString(), id, iCategory);
            ViewBag.drpSubject = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.Subject.ToString());
            ViewBag.drpCategory1 = CommonRepository.GetLookups(moduleCode: AppConstants.LookupType.ExamType.ToString());
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetLookups(string moduleCode)
        {
            var lookupList = dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive).ToList().OrderByDescending(x => x.LookupId).OrderBy(l => l.Text);
            return Json(lookupList, JsonRequestBehavior.AllowGet);
        }
        private void AddEditLookup(string text, string value, string moduleCode, int? id, string parent = "")
        {
            if (dbContext.Lookup.Any(x => x.LookupId == id && x.IsActive))
            {
                var lookupObj = dbContext.Lookup.FirstOrDefault(x => x.LookupId == id && x.ModuleCode == moduleCode && x.IsActive);
                if (lookupObj != null)
                {
                    lookupObj.Text = text;
                    lookupObj.Value = value;
                    lookupObj.Parent = parent;
                    dbContext.Entry(lookupObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(parent))
                {
                    Lookup lookup = new Lookup();
                    lookup.Text = text;
                    lookup.Value = value;
                    lookup.Description = text;
                    lookup.ModuleCode = moduleCode;
                    lookup.Order = dbContext.Lookup.Where(x => x.ModuleCode == moduleCode).ToList().Count + 1;
                    lookup.IsActive = true;
                    lookup.Parent = parent;
                    dbContext.Lookup.Add(lookup);
                    dbContext.SaveChanges();
                }
                else
                {
                    var lookupObj = dbContext.Lookup.FirstOrDefault(x => x.ModuleCode == moduleCode && x.Value == value && x.Parent == parent);
                    if (lookupObj == null)
                    {
                        Lookup lookup = new Lookup();
                        lookup.Text = text;
                        lookup.Value = value;
                        lookup.Description = text;
                        lookup.ModuleCode = moduleCode;
                        lookup.Order = dbContext.Lookup.Where(x => x.ModuleCode == moduleCode).ToList().Count + 1;
                        lookup.IsActive = true;
                        lookup.Parent = parent;
                        dbContext.Lookup.Add(lookup);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        lookupObj.IsActive = true;
                        dbContext.Entry(lookupObj).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        [HttpGet]
        public ActionResult DeleteLookup(int? id, string moduleCode)
        {
            var lookupObj = dbContext.Lookup.FirstOrDefault(x => x.LookupId == id && x.ModuleCode == moduleCode);
            if (lookupObj != null)
            {
                lookupObj.IsActive = false;
                dbContext.Entry(lookupObj).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }
    }
}