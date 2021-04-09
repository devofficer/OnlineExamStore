using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LinqKit;
using Microsoft.Ajax.Utilities;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using PagedList;
using WebGrease.Css.Extensions;
using System.Threading;

namespace OnlineExam.Controllers
{
    //[Authorize(Roles = "Admin, StaffAdmin, Student, Teacher")]
    [AuthorizeUser(Roles = "Admin, StaffAdmin, Student, Teacher")]
    public class QuestionPaperController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            return View();
        }
        // GET: /QuestionPaper/
        [HttpGet]
        public ActionResult Index(int? page, string type = "", string opType = "#unAttempted-tab")
        {
            IList<QuestionPaperListViewModel> questionPapars = new List<QuestionPaperListViewModel>();
            var classes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());
            var selectListItems = classes as SelectListItem[] ?? classes.ToArray();
            if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
            {
                questionPapars = QuestionPaperRepository.GetAll("", "", "", string.Empty, CustomClaimsPrincipal.Current.UserId, null);
                SetCBTTypes(questionPapars);
            }
            else
            {
                var firstOrDefault = selectListItems.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var userClassName = firstOrDefault.Value;
                    questionPapars = QuestionPaperRepository.GetAll(userClassName, "", "", opType, CustomClaimsPrincipal.Current.UserId, type);
                    SetCBTTypes(questionPapars);
                }
            }
            return Request.IsAjaxRequest()
               ? (ActionResult)PartialView("_QuestionPaperList", new QuestionFilterViewModel
               {
                   IsAttempted = opType != "#unAttempted-tab",
                   Classes = selectListItems,
                   QuestionPapars = questionPapars.ToPagedList(page ?? 1, 50)
               })
               : View(new QuestionFilterViewModel
               {
                   IsAttempted = opType != "#unAttempted-tab",
                   Classes = selectListItems,
                   QuestionPapars = questionPapars.ToPagedList(page ?? 1, 50)
               });
        }

        private void SetCBTTypes(IList<QuestionPaperListViewModel> questionPapars)
        {
            ViewBag.TotalAdminCBT = questionPapars.Where(x => x.CBTType == CBTType.Admin.ToString()).ToList().Count;
            ViewBag.TotalCustomCBT = questionPapars.Where(x => x.CBTType == CBTType.Custom.ToString()).ToList().Count;
            ViewBag.TotalTeacherCBT = questionPapars.Where(x => x.CBTType == CBTType.Teacher.ToString()).ToList().Count;
        }

        [HttpPost]
        public PartialViewResult Index(QuestionFilterViewModel questionFilterViewModel, int? page, string type="", string opType = "#unAttempted-tab")
        {
            questionFilterViewModel.PageIndex = page ?? 1;
            opType = "#" + opType;
            questionFilterViewModel.IsAttempted = opType != "#unAttempted-tab";
            var questionPapars = QuestionPaperRepository.GetAll(
                questionFilterViewModel.SelectedClass,
                questionFilterViewModel.SelectedCategory == "Select Category" ? string.Empty : questionFilterViewModel.SelectedCategory,
                questionFilterViewModel.SelectedSubject == "Select Subject" ? string.Empty : questionFilterViewModel.SelectedSubject,
                opType, CustomClaimsPrincipal.Current.UserId, type);

            questionFilterViewModel.QuestionPapars = questionPapars.ToPagedList(questionFilterViewModel.PageIndex, questionFilterViewModel.PageSize);
            SetCBTTypes(questionPapars);
            return PartialView("_QuestionPaperListV2", questionFilterViewModel);
        }

        [HttpGet]
        public JsonResult GetCategoriesByClassType(string classType)
        {
            if (string.IsNullOrWhiteSpace(classType))
                return Json(HttpNotFound());

            var categoryList = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), classType);
            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubjectsByCategoryType(string categoryType)
        {
            if (string.IsNullOrWhiteSpace(categoryType))
                return Json(HttpNotFound());

            var subjectList = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), categoryType);
            return Json(subjectList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return DefaultQuestionList(null);
        }

        private ActionResult DefaultQuestionList(IList<QuestionListViewModel> questions)
        {
            var classes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());
            var questionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());
            var selectListItems = classes as SelectListItem[] ?? classes.ToArray();
           // DONT LOAD QUESTION ON PAGE LOAD
            //if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
            //{
            //    if (questions == null)
            //        questions = QuestionBankRepository.GetQuestions("", "", "", "", null, true, "", CustomClaimsPrincipal.Current.IsACDAStoreUser);
            //}
            //else
            //{
            //    var firstOrDefault = selectListItems.FirstOrDefault();
            //    if (firstOrDefault != null)
            //    {
            //        var userClassName = firstOrDefault.Value;
            //        if (questions == null)
            //            questions = QuestionBankRepository.GetQuestions(userClassName, "", "", "", null, true, CustomClaimsPrincipal.Current.UserId, false);
            //    }
            //}
            return View(new QuestionFilterViewModel { Classes = selectListItems, Formats = questionFormats, Questions = new List<QuestionListViewModel>() });
        }


        public ActionResult GetQuestions()
        {
            IList<QuestionListViewModel> questions = QuestionBankRepository.GetQuestions("", "", "", "","", null, true, "",
                CustomClaimsPrincipal.Current.IsACDAStoreUser);
            return
                Json(questions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(QuestionFilterViewModel questionFilterViewModel, List<QuestionListViewModel> questionListViewModel, int? page, string command)
        {
            IList<QuestionListViewModel> questions = null;
            try
            {
                var topics = string.Empty;
                if (questionFilterViewModel.SelectedTopic != null)
                {
                    topics = string.Join("|", questionFilterViewModel.SelectedTopic);
                }

                questionFilterViewModel.PageIndex = page ?? 1;
                if (questionFilterViewModel.SelectedClass == "Select Class")
                {
                    questionFilterViewModel.SelectedClass = "";
                }
                if (questionFilterViewModel.SelectedCategory == "Select Category")
                {
                    questionFilterViewModel.SelectedCategory = "";
                }
                if (questionFilterViewModel.SelectedSubject == "Select Subject")
                {
                    questionFilterViewModel.SelectedSubject = "";
                }

                if (command == "Create")
                {
                    #region CREATE
                    if (string.IsNullOrWhiteSpace(questionFilterViewModel.Name))
                    {
                        ModelState.AddModelError("", "Question Paper name is required.");
                    }
                    if (string.IsNullOrWhiteSpace(questionFilterViewModel.SelectedClass))
                    {
                        ModelState.AddModelError("", "Class Name is required.");
                    }
                    if (string.IsNullOrWhiteSpace(questionFilterViewModel.SelectedCategory))
                    {
                        ModelState.AddModelError("", "Exam Name is required.");
                    }
                    if (string.IsNullOrWhiteSpace(questionFilterViewModel.SelectedSubject))
                    {
                        ModelState.AddModelError("", "Subject is required.");
                    }
                    //if (questionFilterViewModel.SelectedMinute < 5)
                    //{
                    //    ModelState.AddModelError("", "Duration is required.");
                    //}
                    if (questionListViewModel == null || questionListViewModel.Count == 0)
                    {
                        ModelState.AddModelError("", "Please select question(s).");
                    }

                    if (ModelState.IsValid)
                    {
                        var questionPaperDuration = 0;
                        if (questionFilterViewModel.SelectedMinute > 0)
                        {
                            questionPaperDuration = Convert.ToInt32(
                                TimeSpan.FromMinutes(Convert.ToDouble(questionFilterViewModel.SelectedMinute))
                                    .TotalSeconds);
                        }
                        else
                        {
                            if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
                            {
                                questionPaperDuration = questionListViewModel.Aggregate(questionPaperDuration,
                                    (current, item) => current + item.DurationInSecond);
                            }
                        }
                        var questionPaper = new QuestionPaper
                        {
                            Name = questionFilterViewModel.Name,
                            ClassName = questionFilterViewModel.SelectedClass,
                            ExamName = questionFilterViewModel.SelectedCategory,
                            Subject = questionFilterViewModel.SelectedSubject,
                            IsTrial = questionFilterViewModel.IsTrial,
                            IsOnline = questionFilterViewModel.IsOnline,
                            Minute = questionPaperDuration,
                            IsActive = true,
                            CreatedBy = CustomClaimsPrincipal.Current.UserId,
                            CreatedOn = DateTime.Now
                        };

                        /* FIND SECTIONS FOR SELECTED QUESTION LIST */
                        var subjects = db.Sections.ToList();

                        // ADMIN  USER
                        if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
                        {
                            questionPaper.Type = CBTType.Admin.ToString();
                            #region ADMIN USER
                            var selectedSubjects = questionListViewModel.Select(s => new { s.Subject }).ToList().Distinct();
                            var result = (from s in subjects
                                          join ss in selectedSubjects
                                              on s.Name equals ss.Subject
                                          select new
                                          {
                                              s.Id,
                                              s.Name
                                          }).ToList();

                            foreach (var item in questionListViewModel)
                            {
                                var section = result.FirstOrDefault(e => e.Name == item.Subject);
                                questionPaper.QuestionPaperMappings.Add(new QuestionPaperMapping
                                {
                                    QuestionBankId = item.Id,
                                    SectionId = section.Id
                                });
                            }
                            #endregion
                        }
                        else
                        {
                            if(CustomClaimsPrincipal.Current.CurrentRole == "Teacher")
                            {
                                questionPaper.Type = CBTType.Teacher.ToString();
                            }
                            else
                            {
                                questionPaper.Type = CBTType.Custom.ToString();
                            }
                            // NON- ADMIN USER
                            #region NON ADMIN USER
                            int customCBTCounter = 0;
                            questionPaper.IsOnline = questionFilterViewModel.IsOnline = true;
                            if (questionFilterViewModel.SelectedNoOfQuestions == null)
                            {
                                customCBTCounter = CommonRepository.GetCustomCBTCounter();
                            }
                            else
                            {
                                customCBTCounter = Convert.ToInt32(questionFilterViewModel.SelectedNoOfQuestions);
                            }

                            var randomQuestions = QuestionBankRepository.GetRandomQuestions(questionFilterViewModel.SelectedClass,
                                                    questionFilterViewModel.SelectedCategory,
                                                    questionFilterViewModel.SelectedSubject,
                                                    topics,
                                                    questionFilterViewModel.SelectedFormat,
                                                    questionFilterViewModel.SelectedMark,
                                                    questionFilterViewModel.IsOnline, customCBTCounter); //db.QuestionBank.OrderBy(q => Guid.NewGuid()).Take(customCBTCounter).Select(x => new { QuestionId = x.Id, Duration = x.DurationInSecond }).ToList();
                            if (randomQuestions == null || randomQuestions.Count == 0)
                            {
                                return Json(new { success = false, errors = "Questions doesn't exists." });
                            }

                            foreach (var item in randomQuestions)
                            {
                                var section = subjects.FirstOrDefault(e => e.Name == questionFilterViewModel.SelectedSubject);
                                questionPaper.QuestionPaperMappings.Add(new QuestionPaperMapping
                                {
                                    QuestionBankId = item.QuestionId,
                                    SectionId = section.Id
                                });
                            }
                            questionPaperDuration = randomQuestions.Aggregate(questionPaperDuration,
                                    (current, item) => current + item.DurationInSecond);

                            questionPaper.Minute = questionPaperDuration;
                            #endregion
                        }
                        db.QuestionPapers.Add(questionPaper);
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { success = false, errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });
                    }
                    #endregion
                }
                else if (command == "Search")
                {
                    #region SEARCH
                    
                    if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
                    {
                        questions = QuestionBankRepository.GetQuestions(
                            questionFilterViewModel.SelectedClass,
                            questionFilterViewModel.SelectedCategory,
                            questionFilterViewModel.SelectedSubject,
                            topics,
                            questionFilterViewModel.SelectedFormat,
                            questionFilterViewModel.SelectedMark,
                            questionFilterViewModel.IsOnline, "", CustomClaimsPrincipal.Current.IsACDAStoreUser);
                    }
                    else
                    {
                        questionFilterViewModel.IsOnline = true;
                        questions = QuestionBankRepository.GetQuestions(
                            questionFilterViewModel.SelectedClass,
                            questionFilterViewModel.SelectedCategory,
                            questionFilterViewModel.SelectedSubject,
                            topics,
                            questionFilterViewModel.SelectedFormat,
                            questionFilterViewModel.SelectedMark,
                            questionFilterViewModel.IsOnline, CustomClaimsPrincipal.Current.UserId, false);
                    }
                    //return Json(new { success = true, errors = "", output = questions }, JsonRequestBehavior.AllowGet);
                    return new JsonResult()
                    {
                        Data = new { success = true, errors = "", output = questions },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };

                    #endregion
                }
                return Json(new { success = true, errors = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = AppConstants.FailureText });
            }
        }


        // GET: /QuestionPaper/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var questionPaperObj = db.QuestionPapers.Include("QuestionPaperMappings").Include("QuestionPaperMappings.QuestionBank").FirstOrDefault(x => x.Id == id);
            if (questionPaperObj == null)
            {
                return HttpNotFound();
            }

            //var classes = db.Lookup.Where(x => x.ModuleCode == Enums.LookupType.ClassType.ToString() && x.IsActive);
            //var exams = db.Lookup.Where(x => x.ModuleCode == Enums.LookupType.ExamType.ToString() && x.IsActive);
            //var subject = db.Lookup.Where(x => x.ModuleCode == Enums.LookupType.Subject.ToString() && x.IsActive);
            var questionFormat = db.Lookup.Where(x => x.ModuleCode == Enums.LookupType.QuestionFormatType.ToString() && x.IsActive);

            foreach (var item in questionPaperObj.QuestionPaperMappings)
            {
                //var classObj = classes.FirstOrDefault(x => x.Value == item.QuestionBank.ClassName);
                //if (classObj != null)
                //    item.QuestionBank.ClassName = classObj.Text;
                //var examObj = exams.FirstOrDefault(x => x.Value == item.QuestionBank.ExamName);
                //if (examObj != null)
                //    item.QuestionBank.ExamName = examObj.Text;
                //var subjectObj = subject.FirstOrDefault(x => x.Value == item.QuestionBank.Subject);
                //if (subjectObj != null)
                //    item.QuestionBank.Subject = subjectObj.Text;
                var questionFormatObj = questionFormat.FirstOrDefault(x => x.Value == item.QuestionBank.QuestionFormat);
                if (questionFormatObj != null)
                    item.QuestionBank.QuestionFormat = questionFormatObj.Text;
            }

            var v = new QuestionPaperIndexViewModel
            {
                Id = questionPaperObj.Id,
                Name = questionPaperObj.Name,
                Minute = questionPaperObj.Minute,
                IsActive = questionPaperObj.IsActive,
                CreatedBy = questionPaperObj.CreatedBy,
                CreatedOn = questionPaperObj.CreatedOn,
                QuestionPaperMappings = questionPaperObj.QuestionPaperMappings
            };

            return View(v);
        }

        // POST: /QuestionPaper/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(QuestionPaperIndexViewModel questionPaperIndexViewModel)
        {
            if (string.IsNullOrWhiteSpace(questionPaperIndexViewModel.Name))
            {
                ModelState.AddModelError("", "Question Paper name is Required.");
            }
            if (ModelState.IsValid)
            {
                var questionPaperObj = db.QuestionPapers.FirstOrDefault(q => q.Id == questionPaperIndexViewModel.Id);
                if (questionPaperObj != null)
                {
                    questionPaperObj.Name = questionPaperIndexViewModel.Name;
                    if (questionPaperObj.Minute != null || questionPaperObj.Minute > 0)
                    {
                        questionPaperObj.Minute = Convert.ToInt32(
                                    TimeSpan.FromMinutes(Convert.ToDouble(questionPaperIndexViewModel.Minute))
                                        .TotalSeconds);
                    }
                    questionPaperObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                    questionPaperObj.ModifiedOn = DateTime.Now;
                }
                db.Entry(questionPaperObj).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index").WithSuccess(AppConstants.SuccessMessageText);
            }
            return View(questionPaperIndexViewModel);
        }

        // GET: /QuestionPaper/Delete/5
        public async Task<ActionResult> Delete(int? id, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionPaper questionPaperObj = await db.QuestionPapers.FindAsync(id);
            if (questionPaperObj == null)
            {
                return HttpNotFound();
            }
            return View(questionPaperObj);
        }

        // POST: /QuestionPaper/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            QuestionPaper questionPaperObj = await db.QuestionPapers.FindAsync(id);
            if (questionPaperObj != null)
            {
                questionPaperObj.IsActive = false;
                db.Entry(questionPaperObj).State = EntityState.Modified;
            }
            //db.QuestionPapers.Remove(questionpaper);
            await db.SaveChangesAsync();

            var strType = Request.Form["strType"];

            if (strType == "Teacher")
            {
                return RedirectToAction("Index",new { type="Teacher"});
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteMapping(int? id, int paperId, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var questionPaperMapingObj =  db.QuestionPaperMappings.FirstOrDefault(x=>x.QuestionBankId == id && x.QuestionPaperId== paperId);
            if (questionPaperMapingObj == null)
            {
                return HttpNotFound();
            }
            db.QuestionPaperMappings.Remove(questionPaperMapingObj);
            db.SaveChanges();

            if (type == "Teacher")
            {
                return RedirectToAction("Index", new { type = "Teacher" });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
