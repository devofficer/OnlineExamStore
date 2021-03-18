using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Infrastructure;
using OnlineExam.Repositories;

namespace OnlineExam.Controllers
{
    public class CascadingDropDownController : Controller
    {
        //
        // GET: /CascadingDropDown/
        public ActionResult Index()
        {
            var classes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString());
            ViewBag.Classes = classes;
            return View();
        }
        [HttpGet]
        public JsonResult GetClassCategoryList(string classType)
        {
            if (string.IsNullOrWhiteSpace(classType))
                return Json(HttpNotFound());

            var categoryList = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), classType);
            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSubjectCategoryList(string categoryType)
        {
            if (string.IsNullOrWhiteSpace(categoryType))
                return Json(HttpNotFound());

            var subjectList = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), categoryType);
            return Json(subjectList, JsonRequestBehavior.AllowGet);
        }
    }
}