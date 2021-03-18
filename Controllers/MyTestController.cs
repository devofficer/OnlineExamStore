using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using EntityFramework.BulkInsert.Extensions;
using OnlineExam.Helpers;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using OnlineExam.Infrastructure;
using OnlineExam.Utils;

namespace OnlineExam.Controllers
{
    [AuthorizeUser()]
    public class MyTestController : Controller
    {
        List<QuestionViewModel> questionList = null;
        private ApplicationDbContext db = new ApplicationDbContext();

        public MyTestController()
        {
            questionList = new List<QuestionViewModel>();
        }


        public ActionResult ShowResult(int id)
        {
            var testResultViewModel = new TestResultViewModel();
            var questionPaperObj = db.QuestionPapers.Include("QuestionPaperMappings").FirstOrDefault(x => x.Id == id);

            var attemptedQuestionPaparObj = db.AttemptedQuestionPapars.Include("AttemptedQuestions")
                .OrderByDescending(q => q.Id).ThenByDescending(q => q.UserId)
                .FirstOrDefault(x => (x.Id == id || x.QuestionPaparId == id) && x.UserId == CustomClaimsPrincipal.Current.UserId);
            if (attemptedQuestionPaparObj != null && questionPaperObj != null && questionPaperObj.QuestionPaperMappings != null)
            {
                testResultViewModel.QuestionPaparId = attemptedQuestionPaparObj.QuestionPaparId;
                testResultViewModel.Status = attemptedQuestionPaparObj.Status;
                testResultViewModel.TimeTakenInMinutes = attemptedQuestionPaparObj.TimeTakenInMinutes;
                testResultViewModel.QuestionPaparName = attemptedQuestionPaparObj.QuestionPaparName;
                testResultViewModel.TotalCorrectedQuestions = attemptedQuestionPaparObj.TotalCorrectedAnswered;
                testResultViewModel.TotalInCorrectQuestions = attemptedQuestionPaparObj.TotalInCorrectedAnswered;
                testResultViewModel.TotalMarks = attemptedQuestionPaparObj.TotalMarks;
                testResultViewModel.TotalObtainedMarks = attemptedQuestionPaparObj.TotalObtainedMarks;
                testResultViewModel.TotalQuestions = questionPaperObj.QuestionPaperMappings.Count;
                testResultViewModel.TotalAttemptedQuestions = attemptedQuestionPaparObj.TotalCorrectedAnswered +
                                                              attemptedQuestionPaparObj.TotalInCorrectedAnswered;
                testResultViewModel.FormatType = attemptedQuestionPaparObj.FormatType;

                testResultViewModel.TotalUnAttemptedQuestions = testResultViewModel.TotalQuestions - testResultViewModel.TotalAttemptedQuestions;

                testResultViewModel.TestResultQuestionViewModel = (from aq in db.AttemptedQuestions
                                                                   join qb in db.QuestionBank
                                                                   on aq.QuestionId equals qb.Id
                                                                   //on new { aq.QuestionId, aq.ParentId} equals new { qb.Id}
                                                                   where aq.AttemptedQuestionPaparId == attemptedQuestionPaparObj.Id
                                                                   //&& (aq.ParentId == null || aq.ParentId > 0)

                                                                   select new TestResultQuestionViewModel()
                                                                   {
                                                                       AttemptedQuestionPaparId = aq.AttemptedQuestionPaparId,
                                                                       QuestionId = aq.QuestionId,
                                                                       SelectedAnsOption = aq.SelectedAnsOption,
                                                                       IsCorrectAnswer = aq.IsCorrectAnswer,
                                                                       Description = qb.Decription,
                                                                       ImagePath = qb.ImagePath,
                                                                       ParentQuestion = db.QuestionBank.FirstOrDefault(x => x.Id == aq.ParentId) != null ? db.QuestionBank.FirstOrDefault(x => x.Id == aq.ParentId).Decription : string.Empty,
                                                                       OptionA = qb.OptionA,
                                                                       OptionB = qb.OptionB,
                                                                       OptionC = qb.OptionC,
                                                                       OptionD = qb.OptionD,
                                                                       OptionE = qb.OptionE,
                                                                       AnswerOption = qb.AnswerOption,
                                                                       AnswerDescription = qb.AnswerDescription,
                                                                       Mark = qb.Mark
                                                                   }).ToList();

            }
            return View(testResultViewModel);
        }

        [HttpGet]
        public ActionResult Index(int id)
        {
            QuestionViewModel questionViewModel = Session["questionViewModel"] != null ? (QuestionViewModel)Session["questionViewModel"] : new QuestionViewModel();
            var ViewquestionViewModel = LoadQuestionList(id,
                                                         questionViewModel.Command,
                                                         index: questionViewModel.PageIndex == 0 ? 1 : questionViewModel.PageIndex,
                                                         attemptedQuestionPaparId: questionViewModel.AttemptedQuestionPaparId) ?? new QuestionViewModel();

            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_DisplayQuestion", ViewquestionViewModel) : View(ViewquestionViewModel);

        }

        private QuestionViewModel LoadQuestionList(int id, string command, int index = 1, int attemptedQuestionPaparId = 0)
        {
            TempData["questionList"] =
                questionList = (from q in db.QuestionPaperMappings.Include("QuestionBank").Where(x => x.QuestionPaperId == id && (x.QuestionBank.ParentId == null || x.QuestionBank.ParentId == 0))
                                select new QuestionViewModel
                                {
                                    Id = q.QuestionBank.Id,
                                    QuestionId = q.QuestionBank.QuestionId,
                                    Description = q.QuestionBank.Decription,
                                    OptionA = q.QuestionBank.OptionA,
                                    OptionB = q.QuestionBank.OptionB,
                                    OptionC = q.QuestionBank.OptionC,
                                    OptionD = q.QuestionBank.OptionD,
                                    OptionE = q.QuestionBank.OptionE,
                                    Mark = q.QuestionBank.Mark,
                                    AnswerOption = q.QuestionBank.AnswerOption,
                                    FormatType = q.QuestionBank.QuestionFormat,
                                    ImagePath = "~/images/QuestionImages/Q" + q.QuestionBank.QuestionId.ToString() + ".png"
                                }).ToList<QuestionViewModel>();
            var questionViewModel = new QuestionViewModel();
            if (questionList != null)
            {
                if (command == "Next")
                {
                    if (questionList.Skip(index).Any())
                    {
                        questionViewModel = questionList.Skip(index).First();
                        questionViewModel.PageIndex = index + 1;
                    }
                }
                else if (command == "Previous")
                {
                    index--;
                    if (questionList.Skip(index).Any())
                    {
                        questionViewModel = questionList.Take(index).Last();
                        questionViewModel.PageIndex = index;
                    }
                }
                else
                {
                    questionViewModel = questionList.FirstOrDefault();
                }
                if (db.AttemptedQuestions.Any(x => x.QuestionId == questionViewModel.Id))
                {
                    var firstOrDefault = db.AttemptedQuestions.FirstOrDefault(x => x.QuestionId == questionViewModel.Id && x.CreatedBy == CustomClaimsPrincipal.Current.UserId &&
                                                                                   x.AttemptedQuestionPaparId == attemptedQuestionPaparId);
                    if (firstOrDefault != null)
                        questionViewModel.SelectedOptionA = firstOrDefault.SelectedAnsOption;
                }

                if (questionViewModel != null)
                {
                    questionViewModel.ChildQuestions = (from x in db.QuestionBank.Where(x => x.ParentId == questionViewModel.Id)
                                                        join aq in db.AttemptedQuestions
                                                        .Where(x => x.CreatedBy == CustomClaimsPrincipal.Current.UserId &&
                                                            x.AttemptedQuestionPaparId == attemptedQuestionPaparId)
                                                        on x.Id equals aq.QuestionId into gj
                                                        from resultSet in gj.DefaultIfEmpty()
                                                        select new QuestionViewModel
                                                        {
                                                            Id = x.Id,
                                                            QuestionId = x.QuestionId,
                                                            Description = x.Decription,
                                                            OptionA = x.OptionA,
                                                            OptionB = x.OptionB,
                                                            OptionC = x.OptionC,
                                                            OptionD = x.OptionD,
                                                            OptionE = x.OptionE,
                                                            Mark = x.Mark,
                                                            AnswerOption = x.AnswerOption,
                                                            ImagePath = x.ImagePath,
                                                            SelectedOptionA = (resultSet == null) ? string.Empty : resultSet.SelectedAnsOption
                                                        }).ToList();

                    var questionPaperObj = db.QuestionPapers.FirstOrDefault(x => x.Id == id);
                    if (questionPaperObj != null)
                    {
                        questionViewModel.Minute = Convert.ToInt32(questionPaperObj.Minute);
                        questionViewModel.QuestionPaparName = questionPaperObj.Name;
                    }

                    var atemptedQuestionPaparObj = db.AttemptedQuestionPapars.FirstOrDefault(x => x.Id == attemptedQuestionPaparId);
                    questionViewModel.AttemptedQuestionPaparId = atemptedQuestionPaparObj != null ? atemptedQuestionPaparObj.Id : attemptedQuestionPaparId;

                    var b =
                        (from q in questionList join child in db.QuestionBank on q.Id equals child.ParentId select q)
                            .ToList().Count;

                    questionViewModel.QuestionPaparId = id;
                    questionViewModel.TotalQuestion = questionList.Count + b > 0 ? questionList.Count + b - 1 : questionList.Count;
                    questionViewModel.QuestionCount = questionList.Count;
                    questionViewModel.TotalMarks = questionList.Sum(x => x.Mark);
                    //questionViewModel.TotalQuestion = questionList.Count;
                    questionViewModel.IsPreButtonEnabled = questionViewModel.PageIndex > 1;
                    questionViewModel.IsNxtButtonEnabled = questionViewModel.PageIndex != questionList.Count;
                }
            }
            return questionViewModel;
        }

        [HttpPost]
        public ActionResult Index(QuestionViewModel questionViewModel)
        {
            Session["questionViewModel"] = questionViewModel;
            var attemptedQuestionPaparObj = new AttemptedQuestionPapar();
            var attemptedQuestionObj = new AttemptedQuestion();

            if (questionViewModel.Command == "Next" || questionViewModel.Command == "SubmitTest")
            {
                if (!db.AttemptedQuestionPapars.Any(x => x.QuestionPaparId == questionViewModel.QuestionPaparId
                    && x.Id == questionViewModel.AttemptedQuestionPaparId))
                {
                    //INSERT ATTEMPTED QUESTION PAPAR
                    #region INIT
                    attemptedQuestionPaparObj.QuestionPaparId = questionViewModel.QuestionPaparId;
                    attemptedQuestionPaparObj.UserId = CustomClaimsPrincipal.Current.UserId;
                    attemptedQuestionPaparObj.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                    attemptedQuestionPaparObj.CreatedOn = DateTime.Now;
                    attemptedQuestionPaparObj.TotalQuestions = questionViewModel.TotalQuestion;
                    attemptedQuestionPaparObj.TotalMarks = questionViewModel.TotalMarks;
                    attemptedQuestionPaparObj.TimeTakenInMinutes = questionViewModel.Minute;//in seconds

                    using (var dbContext = new ApplicationDbContext())
                    {
                        if (dbContext.AttemptedQuestionPapars.Any(x => x.QuestionPaparId == questionViewModel.QuestionPaparId && !x.IsArchive))
                        {
                            var aqp = dbContext.AttemptedQuestionPapars.FirstOrDefault(x => x.QuestionPaparId == questionViewModel.QuestionPaparId && !x.IsArchive);
                            if (aqp != null)
                            {
                                aqp.IsArchive = true;
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    if (questionViewModel.ChildQuestions != null && questionViewModel.ChildQuestions.Count > 0)
                    {
                        //INSERT ATTEMPTED QUESTIONS
                        var attemptedQuestions = new List<AttemptedQuestion>();
                        foreach (var q in questionViewModel.ChildQuestions)
                        {
                            var firstOrDefault = db.QuestionBank.FirstOrDefault(x => x.Id == q.Id);
                            if (firstOrDefault != null)
                            {
                                attemptedQuestionObj = new AttemptedQuestion();
                                attemptedQuestionObj.ParentId = questionViewModel.Id;
                                attemptedQuestionObj.QuestionId = q.Id;
                                attemptedQuestionObj.SelectedAnsOption = q.SelectedOptionA;
                                attemptedQuestionObj.AnswerOption = firstOrDefault.AnswerOption;
                                attemptedQuestionObj.IsCorrectAnswer = firstOrDefault.AnswerOption == q.SelectedOptionA;
                                attemptedQuestionObj.Mark = firstOrDefault.Mark;
                            }
                            attemptedQuestionObj.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                            attemptedQuestionObj.CreatedOn = DateTime.Now;
                            attemptedQuestions.Add(attemptedQuestionObj);
                            attemptedQuestionPaparObj.AttemptedQuestions.Add(attemptedQuestionObj);
                            db.AttemptedQuestions.Add(attemptedQuestionObj);
                        }
                        //db.BulkInsert(attemptedQuestions);
                    }
                    else
                    {
                        //INSERT ATTEMPTED QUESTION

                        var firstOrDefault = db.QuestionBank.FirstOrDefault(x => x.Id == questionViewModel.Id);
                        if (firstOrDefault != null)
                        {
                            attemptedQuestionObj = new AttemptedQuestion();
                            attemptedQuestionObj.QuestionId = questionViewModel.Id;
                            attemptedQuestionObj.SelectedAnsOption = questionViewModel.SelectedOptionA;
                            attemptedQuestionObj.AnswerOption = firstOrDefault.AnswerOption;
                            attemptedQuestionObj.IsCorrectAnswer = firstOrDefault.AnswerOption == questionViewModel.SelectedOptionA;
                            attemptedQuestionObj.Mark = firstOrDefault.Mark;
                        }
                        attemptedQuestionObj.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                        attemptedQuestionObj.CreatedOn = DateTime.Now;
                        attemptedQuestionPaparObj.AttemptedQuestions.Add(attemptedQuestionObj);
                        db.AttemptedQuestions.Add(attemptedQuestionObj);
                    }
                    db.AttemptedQuestionPapars.Add(attemptedQuestionPaparObj);
                    #endregion
                }
                else
                {
                    //UPDATE ATTEMPTED QUESTION PAPAR
                    #region UPDATE
                    attemptedQuestionPaparObj = db.AttemptedQuestionPapars.FirstOrDefault(x => x.QuestionPaparId == questionViewModel.QuestionPaparId
                        && x.Id == questionViewModel.AttemptedQuestionPaparId);
                    if (attemptedQuestionPaparObj != null)
                    {
                        // UPDATE ATTEMPTED QUESTION PAPAR RECORD FOR VARIOUS PROPERTIES
                        attemptedQuestionPaparObj.TimeTakenInMinutes = questionViewModel.Minute;//in seconds
                        attemptedQuestionPaparObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                        attemptedQuestionPaparObj.ModifiedOn = DateTime.Now;

                        db.Entry(attemptedQuestionPaparObj).State = EntityState.Modified;

                        if (questionViewModel.ChildQuestions != null &&
                            questionViewModel.ChildQuestions.Count > 0)
                        {
                            foreach (var childQuestion in questionViewModel.ChildQuestions)
                            {
                                if (db.AttemptedQuestions.Any(x => x.QuestionId == childQuestion.Id && x.AttemptedQuestionPaparId == questionViewModel.AttemptedQuestionPaparId))
                                {
                                    #region UPDATE
                                    var firstOrDefault = db.AttemptedQuestions.FirstOrDefault(x => x.QuestionId == childQuestion.Id && x.AttemptedQuestionPaparId == questionViewModel.AttemptedQuestionPaparId);
                                    if (firstOrDefault != null)
                                    {
                                        firstOrDefault.SelectedAnsOption = childQuestion.SelectedOptionA;
                                        firstOrDefault.IsCorrectAnswer = firstOrDefault.AnswerOption == childQuestion.SelectedOptionA;
                                        firstOrDefault.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                        firstOrDefault.ModifiedOn = DateTime.Now;
                                        db.Entry(firstOrDefault).State = EntityState.Modified;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region INSERT
                                    var firstOrDefault = db.QuestionBank.FirstOrDefault(x => x.Id == childQuestion.Id);
                                    if (firstOrDefault != null)
                                    {
                                        attemptedQuestionObj = new AttemptedQuestion();
                                        attemptedQuestionObj.ParentId = questionViewModel.Id;
                                        attemptedQuestionObj.QuestionId = childQuestion.Id;
                                        attemptedQuestionObj.SelectedAnsOption = childQuestion.SelectedOptionA;
                                        attemptedQuestionObj.AnswerOption = firstOrDefault.AnswerOption;
                                        attemptedQuestionObj.IsCorrectAnswer = firstOrDefault.AnswerOption == childQuestion.SelectedOptionA;
                                        attemptedQuestionObj.Mark = firstOrDefault.Mark;
                                    }
                                    attemptedQuestionObj.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                                    attemptedQuestionObj.CreatedOn = DateTime.Now;
                                    attemptedQuestionPaparObj.AttemptedQuestions.Add(attemptedQuestionObj);
                                    db.AttemptedQuestions.Add(attemptedQuestionObj);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            #region INSERT/UPDATE QUESTION
                            if (db.AttemptedQuestions.Any(x => x.QuestionId == questionViewModel.Id && x.AttemptedQuestionPaparId == questionViewModel.AttemptedQuestionPaparId))
                            {
                                //UPDATE ATTEMPTED QUESTION RECORD
                                attemptedQuestionObj = db.AttemptedQuestions.FirstOrDefault(x => x.QuestionId == questionViewModel.Id && x.AttemptedQuestionPaparId == questionViewModel.AttemptedQuestionPaparId);
                                if (attemptedQuestionObj != null)
                                {
                                    attemptedQuestionObj.SelectedAnsOption = questionViewModel.SelectedOptionA;
                                    attemptedQuestionObj.AnswerOption = questionViewModel.AnswerOption;
                                    attemptedQuestionObj.Mark = questionViewModel.Mark;
                                    attemptedQuestionObj.IsCorrectAnswer = questionViewModel.AnswerOption == questionViewModel.SelectedOptionA;
                                    attemptedQuestionObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                    attemptedQuestionObj.ModifiedOn = DateTime.Now;

                                    db.Entry(attemptedQuestionObj).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                //INSERT ATTEMPTED QUESTION
                                var firstOrDefault = db.QuestionBank.FirstOrDefault(x => x.Id == questionViewModel.Id);
                                if (firstOrDefault != null)
                                {
                                    attemptedQuestionObj = new AttemptedQuestion();
                                    attemptedQuestionObj.QuestionId = questionViewModel.Id;
                                    attemptedQuestionObj.SelectedAnsOption = questionViewModel.SelectedOptionA;
                                    attemptedQuestionObj.AnswerOption = firstOrDefault.AnswerOption;
                                    attemptedQuestionObj.IsCorrectAnswer = firstOrDefault.AnswerOption == questionViewModel.SelectedOptionA;
                                    attemptedQuestionObj.Mark = firstOrDefault.Mark;
                                }
                                attemptedQuestionObj.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                                attemptedQuestionObj.CreatedOn = DateTime.Now;
                                attemptedQuestionPaparObj.AttemptedQuestions.Add(attemptedQuestionObj);
                                db.AttemptedQuestions.Add(attemptedQuestionObj);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                #region SAVE POINT
                using (var txn = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.SaveChanges();
                        db.Entry(attemptedQuestionPaparObj).GetDatabaseValues();
                        questionViewModel.AttemptedQuestionPaparId = attemptedQuestionPaparObj.Id;
                        txn.Commit();
                    }
                    catch
                    {
                        txn.Rollback();
                    }
                }
                #endregion
            }
            if (questionViewModel.Command == "SubmitTest")
            {
                // UPDATE REPORT PROPERTIES IN ATTEMPTED QUESTION PAPAR TABLE

                var excellentPercentage = 65;
                var average = 45;
                var belowAverage = 30;

                attemptedQuestionPaparObj = db.AttemptedQuestionPapars.Include("AttemptedQuestions")
                    .FirstOrDefault(x => x.QuestionPaparId == questionViewModel.QuestionPaparId && x.Id == questionViewModel.AttemptedQuestionPaparId);
                if (attemptedQuestionPaparObj != null && attemptedQuestionPaparObj.AttemptedQuestions != null)
                {
                    //ATTEMPTED QUESTIONS
                    attemptedQuestionPaparObj.TotalQuestions = attemptedQuestionPaparObj.AttemptedQuestions.Count;
                    var questionPaperObj = db.QuestionPapers.Include("QuestionPaperMappings").FirstOrDefault(x => x.Id == questionViewModel.QuestionPaparId);
                    if (questionPaperObj != null)
                    {
                        attemptedQuestionPaparObj.TimeTakenInMinutes = Convert.ToInt32(questionPaperObj.Minute) - questionViewModel.Minute;
                        attemptedQuestionPaparObj.QuestionPaparName = questionPaperObj.Name;
                        attemptedQuestionPaparObj.TotalMarks = attemptedQuestionPaparObj.AttemptedQuestions.Sum(q => q.Mark); ;

                        attemptedQuestionPaparObj.IsCompletelyAttempted = questionPaperObj.QuestionPaperMappings.Count == attemptedQuestionPaparObj.AttemptedQuestions.Count();
                    }
                    attemptedQuestionPaparObj.ModifiedOn = DateTime.Now;
                    attemptedQuestionPaparObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;

                    attemptedQuestionPaparObj.TotalCorrectedAnswered = attemptedQuestionPaparObj.AttemptedQuestions.Count(x => x.IsCorrectAnswer);

                    attemptedQuestionPaparObj.TotalInCorrectedAnswered = attemptedQuestionPaparObj.AttemptedQuestions.Count(x => !x.IsCorrectAnswer);

                    attemptedQuestionPaparObj.TotalObtainedMarks = attemptedQuestionPaparObj.AttemptedQuestions.Where(x => x.IsCorrectAnswer).Sum(q => q.Mark);

                    var percentile = CommonRepository.GetPercentile(attemptedQuestionPaparObj.TotalObtainedMarks,
                        attemptedQuestionPaparObj.TotalMarks);
                    if (percentile >= excellentPercentage)
                    {
                        attemptedQuestionPaparObj.Status = "Excellent";
                    }
                    else if (percentile >= average && percentile < excellentPercentage)
                    {
                        attemptedQuestionPaparObj.Status = "Average";
                    }
                    else if (percentile >= belowAverage && percentile < average)
                    {
                        attemptedQuestionPaparObj.Status = "Below Average";
                    }
                    else
                    {
                        attemptedQuestionPaparObj.Status = "Below Average";
                    }


                    db.Entry(attemptedQuestionPaparObj).State = EntityState.Modified;
                    questionViewModel.AttemptedQuestionPaparId = db.SaveChanges();
                }
            }
            questionViewModel = LoadQuestionList(questionViewModel.QuestionPaparId, questionViewModel.Command, questionViewModel.PageIndex,
            questionViewModel.AttemptedQuestionPaparId);

            //questionViewModel.SelectedOptionA = null;
            //questionViewModel.ChildQuestions = questionViewModelObj.ChildQuestions;

            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_DisplayQuestion", questionViewModel) : View(questionViewModel);
        }

        [HttpPost]
        public ActionResult OnReferesh(int id, string command, int index, int attemptedQuestionPaparId)
        {
            QuestionViewModel questionViewModel = LoadQuestionList(id, "Next", index, attemptedQuestionPaparId);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_DisplayQuestion", questionViewModel) : View(questionViewModel);
        }
       

        [HttpPost]
        public string SaveQuestionError(string discription, string questionId)
        {
            if (!string.IsNullOrWhiteSpace(questionId) && !string.IsNullOrWhiteSpace(discription))
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    dbContext.QuestionErrors.Add(new QuestionError
                    {
                        QuestionId = Convert.ToInt32(questionId),
                        UserId = CustomClaimsPrincipal.Current.UserId,
                        CreatedBy = CustomClaimsPrincipal.Current.UserId,
                        CreatedOn = DateTime.UtcNow,
                        Description= discription,
                        ActionTaken = ActionOnQuestionError.Submitted.ToString()
                    });

                    dbContext.SaveChanges();
                    return AppConstants.SuccessMessageText;
                }
            }
            return AppConstants.FailureText;
        }
    }
}