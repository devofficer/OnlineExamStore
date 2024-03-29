﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using LinqKit;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using PagedList;

namespace OnlineExam.Controllers
{
    //[Authorize(Roles = "Admin, StaffAdmin, Student, Teacher")]
    [AuthorizeUser(Roles = "Admin, StaffAdmin, Student, Teacher")]
    public class QuestionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Question/
        public ActionResult Index(QuestionBankSearchViewModel questionBankSearchViewModel, int? page)
        {
            if (!string.IsNullOrWhiteSpace(questionBankSearchViewModel.ClassName))
            {
                questionBankSearchViewModel.PageIndex = 1;
            }
            else
            {
                questionBankSearchViewModel.ClassName = questionBankSearchViewModel.CurrentFilter;
            }
            questionBankSearchViewModel.PageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


            questionBankSearchViewModel.ClasseTypes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());


            //questionBankSearchViewModel.ExamTypes = CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString());
            // questionBankSearchViewModel.Subjects = CommonRepository.GetLookups(Enums.LookupType.Subject.ToString());
            questionBankSearchViewModel.QuestionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            IList<QuestionBankViewModel> questions = null;


            if (questionBankSearchViewModel.QuestionId > 0)
            {
                questions = QuestionBankRepository.GetQuestionsById(questionBankSearchViewModel.QuestionId);
            }
            else
            {
                questions = QuestionBankRepository
                              .GetAll(questionBankSearchViewModel.ClassName, questionBankSearchViewModel.ExamName, questionBankSearchViewModel.Subject,
                              questionBankSearchViewModel.QuestionFormat, questionBankSearchViewModel.Mark, questionBankSearchViewModel.IsOnline, CustomClaimsPrincipal.Current.UserId, CustomClaimsPrincipal.Current.IsACDAStoreUser);

            }
            IPagedList<QuestionBankViewModel> questionsToReturn = questions.OrderBy(b => b.ClassName).ToPagedList(questionBankSearchViewModel.PageIndex, questionBankSearchViewModel.PageSize);

            if (!string.IsNullOrWhiteSpace(questionBankSearchViewModel.ExamName))
            {
                questionBankSearchViewModel.ExamTypes = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), questionBankSearchViewModel.ClassName);
            }
            if (!string.IsNullOrWhiteSpace(questionBankSearchViewModel.Subject))
            {
                questionBankSearchViewModel.Subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), questionBankSearchViewModel.ExamName);
            }

            var v = new QuestionBankIndexViewModel
            {
                QuestionBankList = questionsToReturn,
                QuestionBankSearchViewModel = questionBankSearchViewModel
            };
            return View(v);
        }

        // GET: /Question/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionBank = await db.QuestionBank.FindAsync(id);
            if (questionBank == null)
            {
                return HttpNotFound();
            }

            var classes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString());
            var examType = CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString());
            var subject = CommonRepository.GetLookups(Enums.LookupType.Subject.ToString());
            var questionFormatType = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            questionBank.ClasseTypes = classes;
            questionBank.ExamTypes = examType;
            questionBank.Subjects = subject;
            questionBank.QuestionFormats = questionFormatType;

            return View(questionBank);
        }

        // GET: /Question/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Marks = new int[] { 1, 2, 3, 4, 5 };
            var questionBank = new QuestionBank();
            //var examType = CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString());
            //var subject = CommonRepository.GetLookups(Enums.LookupType.Subject.ToString());
            questionBank.ClasseTypes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());
            //questionBank.ExamTypes = examType;
            //questionBank.Subjects = subject;
            questionBank.QuestionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            return View(questionBank);
        }

        //[HttpGet]
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

            var subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), categoryType);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTopicsBySubject(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
                return Json(HttpNotFound());

            var topics = CommonRepository.GetTopics(subject);
            return Json(topics, JsonRequestBehavior.AllowGet);
        }
        // POST: /Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //   public ActionResult Create([Bind(Include = "Id,Decription,ImagePath,OptionA,OptionB,OptionC,OptionD,OptionE,AnswerOption,AnswerDescription,IsActive,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] QuestionBank questionbank)
        [ValidateInput(false)]
        public ActionResult Create(QuestionBank questionBank)
        {
           
            if (ModelState.IsValid)
            {
                questionBank.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                questionBank.CreatedOn = DateTime.UtcNow;

                var questionId = db.QuestionBank.Max(x=>x.QuestionId);
                questionId = questionId + 1;
                questionBank.QuestionId = questionId;
                db.QuestionBank.Add(questionBank);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return View(questionBank).WithError(AppConstants.ErrorMessageText);
                }
                return RedirectToAction("Create").WithSuccess("Q."+questionId +" " +AppConstants.SuccessMessageText);
            }
            ViewBag.Marks = new int[] { 1, 2, 3, 4, 5 };
            questionBank.ClasseTypes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString());
            questionBank.ExamTypes = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), questionBank.ClassName);// CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString(), questionBank.ClassName);
            questionBank.Subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), questionBank.ExamName);//CommonRepository.GetLookups(Enums.LookupType.Subject.ToString(), questionBank.ExamName);
            questionBank.QuestionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            return View(questionBank).WithError(messages);
        }

        // GET: /Question/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionBank = await db.QuestionBank.FirstOrDefaultAsync(x => x.QuestionId == id);
            if (questionBank == null)
            {
                questionBank = new QuestionBank();
            }
            questionBank.ClasseTypes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());
            questionBank.ExamTypes = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), questionBank.ClassName);// CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString(), questionBank.ClassName);
            questionBank.Subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), questionBank.ExamName);//CommonRepository.GetLookups(Enums.LookupType.Subject.ToString(), questionBank.ExamName);
            questionBank.QuestionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());
            return View(questionBank);
        }

        // POST: /Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(QuestionBank questionBank)
        {
            if (questionBank == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelState.Remove("ExamName");
            ModelState.Remove("Subject");
            ModelState.Remove("QuestionFormat");
            if (ModelState.IsValid)
            {
                try
                {
                    var questionBankObj = await db.QuestionBank.FirstOrDefaultAsync(x => x.QuestionId == questionBank.QuestionId);
                    if (questionBankObj == null)
                    {
                        return HttpNotFound();
                    }
                    questionBankObj.Decription = questionBank.Decription;
                    questionBankObj.OptionA = questionBank.OptionA;
                    questionBankObj.OptionB = questionBank.OptionB;
                    questionBankObj.OptionC = questionBank.OptionC;
                    questionBankObj.OptionD = questionBank.OptionD;
                    questionBankObj.OptionE = questionBank.OptionE;
                    questionBankObj.AnswerOption = questionBank.AnswerOption;
                    questionBankObj.AnswerDescription = questionBank.AnswerDescription;

                    questionBankObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                    questionBankObj.ModifiedOn = DateTime.UtcNow;
                    db.Entry(questionBankObj).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return View(questionBank).WithError(AppConstants.ErrorMessageText);
                }

                return RedirectToAction("Index").WithSuccess(AppConstants.SuccessMessageText);
            }
            questionBank.ClasseTypes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString());
            questionBank.ExamTypes = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), questionBank.ClassName);// CommonRepository.GetLookups(Enums.LookupType.ExamType.ToString(), questionBank.ClassName);
            questionBank.Subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), questionBank.ExamName);//CommonRepository.GetLookups(Enums.LookupType.Subject.ToString(), questionBank.ExamName);
            questionBank.QuestionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            string messages = string.Join("; ", ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));

            return View(questionBank).WithError(messages);
        }

        // GET: /Question/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionBank questionbank = await db.QuestionBank.FindAsync(id);
            if (questionbank == null)
            {
                return HttpNotFound();
            }
            return View(questionbank);
        }

        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            QuestionBank questionbank = await db.QuestionBank.FindAsync(id);
            db.QuestionBank.Remove(questionbank);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
