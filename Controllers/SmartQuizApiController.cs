using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExam.Helpers;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Web;
using OnlineExam.Utils;
using OnlineExam.Infrastructure;
using OnlineExam.Models.APIViewModels;
using OnlineExam.Repositories;
using System.Configuration;
using Dapper;

namespace OnlineExam.Controllers
{
    [RoutePrefix("api/SmartQuizApi")]
    [AuthorizeUser()]
    public class SmartQuizApiController : ApiController
    {
        #region VERSION 1.0

        /// <summary>
        /// To get user dashboard information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(DashboardViewModel))]
        public HttpResponseMessage GetUserDashboardInfo(string userId)
        {
            //TransactionalInformation transaction;
            string className = "";
            var dashboardViewModel = new DashboardViewModel();
            try
            {
                if (!CustomClaimsPrincipal.Current.IsACDAStoreUser)
                {
                    className = CommonRepository.GetUserClass(userId);
                    if (string.IsNullOrWhiteSpace(className))
                    {
                        dashboardViewModel.ReturnStatus = false;
                        dashboardViewModel.ReturnMessage.Add(AppConstants.ErrorMessageText);
                        var responseError = Request.CreateResponse<DashboardViewModel>(HttpStatusCode.BadRequest, dashboardViewModel);
                        return responseError;
                    }
                }
                using (var dbContext = new ApplicationDbContext())
                {
                    var p1 = new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.VarChar, Value = userId };
                    var p2 = new SqlParameter { ParameterName = "@ClassName", SqlDbType = SqlDbType.VarChar, Value = string.IsNullOrEmpty(className) ? DBNull.Value.ToString() : className };

                    dashboardViewModel = dbContext.Database.SqlQuery<DashboardViewModel>("[dbo].[GetUserDashboardInfo]  @UserId, @ClassName", p1, p2).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLogFile("GetUserDashboardInfo", ex.Message, ex.StackTrace);
                dashboardViewModel.ReturnStatus = false;
                dashboardViewModel.ReturnMessage.Add(AppConstants.ErrorMessageText);
                var responseError = Request.CreateResponse<DashboardViewModel>(HttpStatusCode.BadRequest, dashboardViewModel);
                return responseError;
            }
            var response = Request.CreateResponse<DashboardViewModel>(HttpStatusCode.OK, dashboardViewModel);
            return response;
        }

        /// <summary>
        /// To get question for quiz
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="questionPaperId"></param>
        /// <returns></returns>
        [ResponseType(typeof(SmartQuestionPaperViewModel))]
        public IHttpActionResult Get(int currentIndex, int questionPaperId)
        {
            try
            {
                if (!SessionHelper.IsExisting(CustomClaimsPrincipal.Current.UserId))
                {
                    LoadQuestionPaper(questionPaperId);
                }
                return FindQuestionByIndex(currentIndex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Submit quiz for analayis
        /// </summary>
        /// <param name="postQuestionData"></param>
        /// <returns></returns>
        [ResponseType(typeof(QuestionPaperViewModel))]
        public IHttpActionResult PostQuizData(PostQuestionData postQuestionData)
        {
            if (postQuestionData.isNext)
            {
                SetAnswer(postQuestionData);
            }

            if (postQuestionData.isSubmit)
            {
                // SAVE ATTEMPTED QUESTION PAPER
                return SaveQuestion();
            }
            else
            {
                //Thread.Sleep(1000);
                return FindQuestionByIndex(postQuestionData.currentIndex);
            }
        }

        /// <summary>
        /// To report an error for any question during quiz
        /// </summary>
        /// <param name="discription"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [HttpPost]
        public string PostQuizError(string discription, string questionId)
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
                        Description = discription,
                        ActionTaken = ActionOnQuestionError.Submitted.ToString()
                    });

                    dbContext.SaveChanges();
                    return AppConstants.SuccessMessageText;
                }
            }
            return AppConstants.FailureText;
        }

        #endregion
        #region PRIVATE METHODS
        public SmartQuestionPaperViewModel QuestionPaperObject
        {
            get
            {
                return HttpContext.Current.Session["QuestionPaperObject"] != null ? HttpContext.Current.Session["QuestionPaperObject"] as SmartQuestionPaperViewModel : new SmartQuestionPaperViewModel();
            }
            set
            {

                HttpContext.Current.Session["QuestionPaperObject"] = value;
            }
        }
        private IHttpActionResult SaveQuestion()
        {
            var userId = User.Identity.Name;
            var ld = new List<AnswerQuestion>();
            var userMaxId = 1;
            using (var db = new ApplicationDbContext())
            {
                var f = db.AnswerQuestions.OrderByDescending(r => r.CreatedOn).FirstOrDefault(x => x.UserId == CustomClaimsPrincipal.Current.UserId
                    && x.ParentId == null);
                if (f != null)
                {
                    userMaxId = f.UserMaxId + 1;
                }
                using (var txn = db.Database.BeginTransaction())
                {
                    try
                    {
                        var questionPaperObject = SessionHelper.GetItem<SmartQuestionPaperViewModel>(CustomClaimsPrincipal.Current.UserId);
                        if (questionPaperObject != null)
                        {
                            foreach (var item in questionPaperObject.Questions)
                            {
                                if (item.IsAttempted)
                                {
                                    var d = new AnswerQuestion();
                                    d.TimeTakenInSecond = questionPaperObject.Duration; //SAVING TIME IN SECOND WHICH HAVE TAKEN BY END USER TO FINISH THE QUIZ
                                    d.QuestionPaperId = questionPaperObject.Id;
                                    d.QuestionBankId = item.Id;
                                    d.Answer = item.UserAnswer;
                                    d.UserId = CustomClaimsPrincipal.Current.UserId;//User.Identity.Name;
                                    d.CreatedOn = DateTime.Now;
                                    d.UserMaxId = userMaxId;
                                    d.FormatType = item.FormatType;
                                    db.AnswerQuestions.Add(d);
                                    db.SaveChanges();

                                    if (item.ChildQuestions != null && item.ChildQuestions.Count > 0 && item.FormatType == "CP")
                                    {
                                        foreach (var childItem in item.ChildQuestions)
                                        {
                                            var dm = new AnswerQuestion();
                                            dm.QuestionPaperId = questionPaperObject.Id;
                                            dm.QuestionBankId = childItem.Id;
                                            dm.ParentId = d.Id;//PARENT RECORD ID;
                                            dm.Answer = childItem.UserAnswer;
                                            dm.UserId = CustomClaimsPrincipal.Current.UserId;//User.Identity.Name;
                                            dm.CreatedOn = DateTime.Now;
                                            dm.UserMaxId = 0;
                                            db.AnswerQuestions.Add(dm);
                                        }
                                    }
                                }
                            }
                            db.SaveChanges();

                            /*TO PROCESS QUESTION PAPAR RESULT*/
                            var p1 = new SqlParameter { ParameterName = "@ExcellentPercentage", SqlDbType = SqlDbType.Int, Value = 65 };
                            var p2 = new SqlParameter { ParameterName = "@Average", SqlDbType = SqlDbType.Int, Value = 45 };
                            var p3 = new SqlParameter { ParameterName = "@BelowAverage", SqlDbType = SqlDbType.Int, Value = 30 };
                            var p4 = new SqlParameter { ParameterName = "@QuestionPaparId", SqlDbType = SqlDbType.Int, Value = questionPaperObject.Id };
                            var p5 = new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.VarChar, Value = CustomClaimsPrincipal.Current.UserId };
                            var p6 = new SqlParameter { ParameterName = "@UserMaxId", SqlDbType = SqlDbType.Int, Value = userMaxId };

                            var i = db.Database.ExecuteSqlCommand("ProcessQuestionPaper @ExcellentPercentage, @Average, @BelowAverage, @QuestionPaparId, @UserId, @UserMaxId",
                                p1, p2, p3, p4, p5, p6);

                            if (SessionHelper.IsExisting(CustomClaimsPrincipal.Current.UserId))
                            {
                                SessionHelper.RemoveItem(CustomClaimsPrincipal.Current.UserId);
                            }
                            //HttpContext.Current.Session["QuestionPaperObject"] = null;
                            txn.Commit();
                            return this.Ok("done");
                        }
                        return this.Ok("not done");
                    }
                    catch (Exception ex)
                    {
                        txn.Rollback();
                        Logger.WriteLogFile("SaveQuestion", ex.Message, ex.StackTrace);
                        return this.Ok(ex.Message);
                    }
                }
            }
        }
        private void SetAnswer(PostQuestionData postQuestionData)
        {
            var questionPaperObject = SessionHelper.GetItem<SmartQuestionPaperViewModel>(CustomClaimsPrincipal.Current.UserId);
            if (questionPaperObject != null)
            {
                if (postQuestionData.formatType == "CP")
                {
                    var cQuestions = questionPaperObject.Questions[postQuestionData.preIndex - 1].ChildQuestions;
                    questionPaperObject.Duration = postQuestionData.duration;
                    questionPaperObject.Questions[postQuestionData.preIndex - 1].IsAttempted = true;

                    foreach (var item in postQuestionData.cQuestions)
                    {
                        questionPaperObject.Questions[postQuestionData.preIndex - 1].ChildQuestions.FirstOrDefault(x => x.Id == item.Id).UserAnswer = item.UserAnswer;
                    }
                }
                else
                {
                    if (postQuestionData.preIndex > 0)
                    {
                        questionPaperObject.Duration = postQuestionData.duration;
                        questionPaperObject.Questions[postQuestionData.preIndex - 1].UserAnswer = postQuestionData.userAnswer;
                        questionPaperObject.Questions[postQuestionData.preIndex - 1].IsAttempted = true;
                    }
                }
            }
        }
        private IHttpActionResult FindQuestionByIndex(int currentIndex)
        {
            var questionPaperViewModel = new SmartQuestionPaperViewModel();
            var questionPaperObject = SessionHelper.GetItem<SmartQuestionPaperViewModel>(CustomClaimsPrincipal.Current.UserId);
            if (questionPaperObject != null)
            {
                var question = questionPaperObject.Questions.FirstOrDefault(x => x.Index == currentIndex);
                if (question == null)
                {
                    return this.NotFound();
                }
                questionPaperViewModel.Questions.Add(question);
                questionPaperViewModel.Name = questionPaperObject.Name;
                questionPaperViewModel.TotalMarks = questionPaperObject.TotalMarks;
                questionPaperViewModel.Duration = questionPaperObject.Duration;
                questionPaperViewModel.QuestionCount = questionPaperObject.Questions.Count;
                questionPaperViewModel.Subject = questionPaperObject.Subject;
                questionPaperViewModel.UserId = CustomClaimsPrincipal.Current.UserId;
            }
            return this.Ok(questionPaperViewModel);
        }
        private void LoadQuestionPaper(int questionPaperId)
        {
            using (var db = new ApplicationDbContext())
            {
                // Create a SQL command to execute the sproc
                var dbCommand = db.Database.Connection.CreateCommand();
                dbCommand.CommandText = "[dbo].[GetQuestionPaperById]";
                dbCommand.CommandType = CommandType.StoredProcedure;
                try
                {
                    var p1 = new SqlParameter { ParameterName = "@QuestionPaperId", SqlDbType = SqlDbType.Int, Value = questionPaperId };

                    db.Database.Connection.Open();
                    dbCommand.Parameters.Add(p1);

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        var smartQuestionPaperViewModel = new SmartQuestionPaperViewModel();
                        var questionPaperSummary = reader.MapToList<QuestionPaperSummary>();
                        reader.NextResult();
                        var questionList = reader.MapToList<SmartQuestionViewModel>();
                        reader.NextResult();
                        var childQuestionList = reader.MapToList<SmartQuestionViewModel>();
                        if (questionPaperSummary != null)
                        {
                            smartQuestionPaperViewModel.Id = questionPaperSummary.FirstOrDefault().QuestionPaperId;
                            smartQuestionPaperViewModel.Name = questionPaperSummary.FirstOrDefault().Name;
                            smartQuestionPaperViewModel.Duration = questionPaperSummary.FirstOrDefault().Duration;
                            smartQuestionPaperViewModel.TotalMarks = questionPaperSummary.FirstOrDefault().TotalMarks;
                            smartQuestionPaperViewModel.Subject = questionPaperSummary.FirstOrDefault().Subject;
                        }
                        if (questionList != null)
                        {
                            smartQuestionPaperViewModel.Questions = questionList;
                        }
                        if (childQuestionList != null)
                        {
                            foreach (var item in smartQuestionPaperViewModel.Questions)
                            {
                                var l = childQuestionList.Where(x => x.ParentId != null && x.ParentId == item.Id && x.FormatType == "CP").ToList().Select((c, index) => new SmartQuestionViewModel
                                {
                                    Index = index + 1,
                                    Id = c.Id,
                                    QuestionId = c.QuestionId,
                                    Title = c.Title,
                                    OptionA = c.OptionA,
                                    OptionB = c.OptionB,
                                    OptionC = c.OptionC,
                                    OptionD = c.OptionD,
                                    OptionE = c.OptionE,
                                    FormatType = c.FormatType,
                                    ImagePath = c.ImagePath
                                }).ToList();
                                item.ChildQuestions.AddRange(l);
                            }
                        }
                        SessionHelper.AddItem(smartQuestionPaperViewModel, CustomClaimsPrincipal.Current.UserId);
                    }
                }
                catch (Exception ex)
                {
                    db.Database.Connection.Close();
                    Logger.WriteLogFile("LoadQuestionPaper", ex.Message, ex.StackTrace);
                }
            }

            //using (var db = new ApplicationDbContext())
            //{
            //    var questionPaperObject = db.QuestionPapers.Include("QuestionPaperMappings")
            //          .Include("QuestionPaperMappings.QuestionBank").AsEnumerable()
            //          .Where(x => x.Id == questionPaperId)
            //          .Select(x => new SmartQuestionPaperViewModel
            //          {
            //              Id = x.Id,
            //              Name = x.Name,
            //              Questions = x.QuestionPaperMappings.Select((item, index) => new SmartQuestionViewModel
            //              {
            //                  Index = index + 1,
            //                  Id = item.QuestionBank.Id,
            //                  QuestionId = item.QuestionBank.QuestionId,
            //                  Title = item.QuestionBank.Decription,
            //                  OptionA = item.QuestionBank.OptionA,
            //                  OptionB = item.QuestionBank.OptionB,
            //                  OptionC = item.QuestionBank.OptionC,
            //                  OptionD = item.QuestionBank.OptionD,
            //                  OptionE = item.QuestionBank.OptionE,
            //                  FormatType = item.QuestionBank.QuestionFormat,
            //                  ImagePath = item.QuestionBank.ImagePath != null ? "/images/QuestionImages/Q" + item.QuestionBank.ImagePath + ".png" : ""
            //              }).ToList(),
            //              Duration = (int)x.QuestionPaperMappings.Sum(d => d.QuestionBank.DurationInSecond),
            //              TotalMarks = x.QuestionPaperMappings.Sum(d => d.QuestionBank.Mark)
            //          }).FirstOrDefault();

            //    questionPaperObject.QuestionCount = questionPaperObject.Questions.Count;

            //    foreach (var item in questionPaperObject.Questions)
            //    {
            //        var l = db.QuestionBank.Where(x => x.ParentId != null && x.ParentId == item.Id && x.QuestionFormat == "CP").ToList().Select((c, index) => new SmartQuestionViewModel
            //        {
            //            Index = index + 1,
            //            Id = c.Id,
            //            QuestionId = c.QuestionId,
            //            Title = c.Decription,
            //            OptionA = c.OptionA,
            //            OptionB = c.OptionB,
            //            OptionC = c.OptionC,
            //            OptionD = c.OptionD,
            //            OptionE = c.OptionE,
            //            FormatType = c.QuestionFormat,
            //            ImagePath = "~/images/QuestionImages/Q" + c.QuestionId + ".png"
            //        }).ToList();
            //        item.ChildQuestions.AddRange(l);
            //    }
            //    SessionHelper.AddItem(questionPaperObject, CustomClaimsPrincipal.Current.UserId);
            //}
        }
        private static void LoadQuestionPaparResultSet(int questionPaperId)
        {
            using (var db = new ApplicationDbContext())
            {
                // Create a SQL command to execute the sproc
                var dbCommand = db.Database.Connection.CreateCommand();
                dbCommand.CommandText = "[dbo].[GetQuestionPaperResultSet]";
                dbCommand.CommandType = CommandType.StoredProcedure;

                try
                {
                    var p1 = new SqlParameter { ParameterName = "@ExcellentPercentage", SqlDbType = SqlDbType.Int, Value = 65 };
                    var p2 = new SqlParameter { ParameterName = "@Average", SqlDbType = SqlDbType.Int, Value = 45 };
                    var p3 = new SqlParameter { ParameterName = "@BelowAverage", SqlDbType = SqlDbType.Int, Value = 30 };
                    var p4 = new SqlParameter { ParameterName = "@QuestionPaperId", SqlDbType = SqlDbType.Int, Value = questionPaperId };
                    var p5 = new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.VarChar, Value = CustomClaimsPrincipal.Current.UserId };
                    var p6 = new SqlParameter { ParameterName = "@UserMaxId", SqlDbType = SqlDbType.Int, Value = 0 };

                    db.Database.Connection.Open();
                    dbCommand.Parameters.Add(p1);
                    dbCommand.Parameters.Add(p2);
                    dbCommand.Parameters.Add(p3);
                    dbCommand.Parameters.Add(p4);
                    dbCommand.Parameters.Add(p5);
                    dbCommand.Parameters.Add(p6);

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        var tResult = reader.MapToList<TResult>();
                        reader.NextResult();
                        var tAnswerResult = reader.MapToList<TAnswerResult>();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.Connection.Close();
                    Logger.WriteLogFile("LoadQuestionPaparResultSet", ex.Message, ex.StackTrace);
                }
            }
        }

        #endregion
        #region VERSION 2.0
        /// <summary>
        /// To save Quiz data into database
        /// </summary>
        /// <param name="testViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult PostQuiz(TestViewModel testViewModel)
        {
            var userId = User.Identity.Name;
            var ld = new List<AnswerQuestion>();
            var userMaxId = 1;
            using (var db = new ApplicationDbContext())
            {
                var f = db.AnswerQuestions.OrderByDescending(r => r.CreatedOn).FirstOrDefault(x => x.UserId == CustomClaimsPrincipal.Current.UserId
                    && x.ParentId == null);
                if (f != null)
                {
                    userMaxId = f.UserMaxId + 1;
                }
                using (var txn = db.Database.BeginTransaction())
                {
                    try
                    {
                        var quizViewModel = testViewModel.QuizViewModel;
                        foreach (var item in quizViewModel)
                        {
                            var userOption = item.Option != null ? item.Option.Id.ToString() : "";
                            var d = new AnswerQuestion();
                            d.QuestionPaperId = testViewModel.Id;
                            d.QuestionBankId = item.QuestionId; // Question Bank Id

                            switch (userOption)
                            {
                                case "1":
                                    d.Answer = "A";
                                    break;
                                case "2":
                                    d.Answer = "B";
                                    break;
                                case "3":
                                    d.Answer = "C";
                                    break;
                                case "4":
                                    d.Answer = "D";
                                    break;
                                case "5":
                                    d.Answer = "E";
                                    break;
                                default:
                                    d.Answer = "";
                                    break;
                            }
                            d.UserId = CustomClaimsPrincipal.Current.UserId;//User.Identity.Name;
                            d.CreatedOn = DateTime.Now;
                            d.UserMaxId = userMaxId;
                            d.FormatType = item.Format;
                            db.AnswerQuestions.Add(d);
                            db.SaveChanges();
                        }

                        /*TO PROCESS QUESTION PAPAR RESULT*/
                        //var p1 = new SqlParameter { ParameterName = "@ExcellentPercentage", SqlDbType = SqlDbType.Int, Value = 65 };
                        //var p2 = new SqlParameter { ParameterName = "@Average", SqlDbType = SqlDbType.Int, Value = 45 };
                        //var p3 = new SqlParameter { ParameterName = "@BelowAverage", SqlDbType = SqlDbType.Int, Value = 30 };
                        var p1 = new SqlParameter { ParameterName = "@QuestionPaparId", SqlDbType = SqlDbType.Int, Value = testViewModel.Id };
                        var p2 = new SqlParameter { ParameterName = "@Duration", SqlDbType = SqlDbType.Int, Value = testViewModel.Duration };
                        var p3 = new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.VarChar, Value = CustomClaimsPrincipal.Current.UserId };
                        //var p4 = new SqlParameter { ParameterName = "@UserMaxId", SqlDbType = SqlDbType.Int, Value = userMaxId };

                        var i = db.Database.ExecuteSqlCommand("ProcessQuestionPaperV2 @QuestionPaparId, @Duration, @UserId", p1, p2, p3);

                        txn.Commit();
                        return this.Ok("done");
                    }
                    catch (Exception ex)
                    {
                        txn.Rollback();
                        Logger.WriteLogFile("PostQuiz", ex.Message, ex.StackTrace);
                        return this.Ok(AppConstants.GenericErrorMessageText);
                    }
                }
            }
        }

        /// <summary>
        /// To save data reported during quiz attempted or solution analysis
        /// </summary>
        /// <param name="reportText"></param>
        /// <param name="reportType"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [HttpPost]
        public string PostQuizErrorV2(string reportText, string reportType, string questionId)
        {
            if (!string.IsNullOrWhiteSpace(questionId))
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    dbContext.QuestionErrors.Add(new QuestionError
                    {
                        QuestionId = Convert.ToInt32(questionId),
                        UserId = CustomClaimsPrincipal.Current.UserId,
                        CreatedBy = CustomClaimsPrincipal.Current.UserId,
                        CreatedOn = DateTime.UtcNow,
                        Description = reportText,
                        ReportType = reportType,
                        ActionTaken = ActionOnQuestionError.Submitted.ToString()
                    });

                    dbContext.SaveChanges();
                    return AppConstants.SuccessMessageText;
                }
            }
            return AppConstants.FailureText;
        }

        /// <summary>
        /// Get Quiz by using question paper Id
        /// </summary>
        /// <param name="questionPaperId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetQuizById(int questionPaperId)
        {
            IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var testViewModel = new Test();
            using (var multipleResult = db.QueryMultiple("GetQuestionPaperByIdV2", new { questionPaperId = questionPaperId }, commandType: CommandType.StoredProcedure))
            {
                testViewModel = multipleResult.Read<Test>().SingleOrDefault();
                var sections = multipleResult.Read<OnlineExam.Models.APIViewModels.Section>().ToList();
                var totalQuestions = multipleResult.Read<QuestionViewModelV2>().ToList();
                var childQuestionList = multipleResult.Read<QuestionViewModelV2>().ToList();
                if (testViewModel != null && sections != null)
                {
                    testViewModel.Sections.AddRange(sections);
                }

                if (childQuestionList != null)
                {
                    foreach (var item in totalQuestions)
                    {
                        item.ChildQuestions.AddRange(childQuestionList.Where(q => q.ParentId == item.Id));
                    }
                }

                foreach (var item in testViewModel.Sections)
                {
                    var questions = totalQuestions.Where(x => x.SectionId == item.Id).ToList();
                    foreach (var question in questions)
                    {
                        var questionObj = GetFormatedQuestion(question);
                        // IF FORMAT IS CP, THEN APPEND CHILD QUESTIONS
                        foreach (var childQuestion in question.ChildQuestions)
                        {
                            var childQuestionObj = GetFormatedQuestion(childQuestion);
                            questionObj.ChildQuestions.Add(childQuestionObj);
                        }
                        item.Questions.Add(questionObj);
                    }
                }
            }
            return this.Ok(testViewModel);
        }

        [HttpGet]
        public IHttpActionResult GetLoggedInUserInfo()
        {
            var loggedInUserInfo = new LoggedInUserViewModel
            {
                IsACDAStoreUser = CustomClaimsPrincipal.Current.IsACDAStoreUser,
                UserFullName = CustomClaimsPrincipal.Current.UserFullName,
                UserType = CustomClaimsPrincipal.Current.UserType,
                ClassTypes = CustomClaimsPrincipal.Current.ClassTypes,
                MembershipPlan = CustomClaimsPrincipal.Current.MembershipPlan,
                MembershipPlanCode = CustomClaimsPrincipal.Current.MembershipPlanCode,
                Avatar = CustomClaimsPrincipal.Current.Avatar,
                CurrentRole = CustomClaimsPrincipal.Current.CurrentRole,
                CurrentUserEmail = CustomClaimsPrincipal.Current.CurrentUserEmail,
                UserId = CustomClaimsPrincipal.Current.UserId,
            };
            return this.Ok(loggedInUserInfo);
        }

        private static Question GetFormatedQuestion(QuestionViewModelV2 question)
        {
            var questionObj = new Question
            {
                Id = question.Id,
                Title = question.Title,
                SectionId = question.SectionId,
                Format = question.FormatType,
                Mark = question.Mark,
                Duration = question.Duration,
                Subject = question.Subject,
                ImagePath = question.ImagePath,
                Solution = question.Solution
            };
            if (!string.IsNullOrWhiteSpace(question.OptionA))
            {
                var optionA = new Option { Id = 1, QuestionId = questionObj.Id, Value = question.OptionA };
                questionObj.Options.Add(optionA);
            }
            if (!string.IsNullOrWhiteSpace(question.OptionB))
            {
                var optionB = new Option { Id = 2, QuestionId = questionObj.Id, Value = question.OptionB };
                questionObj.Options.Add(optionB);
            }
            if (!string.IsNullOrWhiteSpace(question.OptionC))
            {
                var optionC = new Option { Id = 3, QuestionId = questionObj.Id, Value = question.OptionC };
                questionObj.Options.Add(optionC);
            }
            if (!string.IsNullOrWhiteSpace(question.OptionD))
            {
                var optionD = new Option { Id = 4, QuestionId = questionObj.Id, Value = question.OptionD };
                questionObj.Options.Add(optionD);
            }
            if (!string.IsNullOrWhiteSpace(question.OptionE))
            {
                var optionE = new Option { Id = 5, QuestionId = questionObj.Id, Value = question.OptionE };
                questionObj.Options.Add(optionE);
            }
            return questionObj;
        }

        /// <summary>
        /// To get Quiz's solution by question paper id
        /// </summary>
        /// <param name="questionPaperId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetQuizSolutionById(int questionPaperId)
        {
            IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var testViewModel = new Test();
            using (var multipleResult = db.QueryMultiple("GetQuestionPaperResultSetV2", new { questionPaperId = questionPaperId, userId = CustomClaimsPrincipal.Current.UserId }, commandType: CommandType.StoredProcedure))
            {
                testViewModel = multipleResult.Read<Test>().FirstOrDefault();
                var sections = multipleResult.Read<OnlineExam.Models.APIViewModels.Section>().ToList();
                var totalQuestions = multipleResult.Read<QuestionViewModelV2>().ToList();
                if (testViewModel != null && sections != null)
                {
                    testViewModel.Sections.AddRange(sections);
                }

                foreach (var item in testViewModel.Sections)
                {
                    var questions = totalQuestions.Where(x => x.SectionId == item.Id).ToList();
                    foreach (var question in questions)
                    {
                        var userOption = "";
                        switch (question.UserOption)
                        {
                            case "A":
                                userOption = "1";
                                break;
                            case "B":
                                userOption = "2";
                                break;
                            case "C":
                                userOption = "3";
                                break;
                            case "D":
                                userOption = "4";
                                break;
                            case "E":
                                userOption = "5";
                                break;
                            default:
                                userOption = "";
                                break;
                        }
                        var questionObj = new Question
                        {
                            Id = question.Id,
                            Title = question.Title,
                            SectionId = question.SectionId,
                            Format = question.FormatType,
                            Mark = question.Mark,
                            Duration = question.Duration,
                            Subject = question.Subject,
                            Solution = question.Solution,
                            ImagePath = question.ImagePath,
                            UserOption = userOption
                        };
                        if (!string.IsNullOrWhiteSpace(question.OptionA))
                        {
                            var optionA = new Option { Id = 1, QuestionId = questionObj.Id, Value = question.OptionA };
                            if (question.AnswerOption == "A")
                                optionA.IsRightOption = true;
                            questionObj.Options.Add(optionA);
                        }
                        if (!string.IsNullOrWhiteSpace(question.OptionB))
                        {
                            var optionB = new Option { Id = 2, QuestionId = questionObj.Id, Value = question.OptionB };
                            if (question.AnswerOption == "B")
                                optionB.IsRightOption = true;
                            questionObj.Options.Add(optionB);
                        }
                        if (!string.IsNullOrWhiteSpace(question.OptionC))
                        {
                            var optionC = new Option { Id = 3, QuestionId = questionObj.Id, Value = question.OptionC };
                            if (question.AnswerOption == "C")
                                optionC.IsRightOption = true;
                            questionObj.Options.Add(optionC);
                        }
                        if (!string.IsNullOrWhiteSpace(question.OptionD))
                        {
                            var optionD = new Option { Id = 4, QuestionId = questionObj.Id, Value = question.OptionD };
                            if (question.AnswerOption == "D")
                                optionD.IsRightOption = true;
                            questionObj.Options.Add(optionD);
                        }
                        if (!string.IsNullOrWhiteSpace(question.OptionE))
                        {
                            var optionE = new Option { Id = 5, QuestionId = questionObj.Id, Value = question.OptionE };
                            if (question.AnswerOption == "E")
                                optionE.IsRightOption = true;
                            questionObj.Options.Add(optionE);
                        }
                        item.Questions.Add(questionObj);
                    }
                }
            }
            return this.Ok(testViewModel);
        }
        #endregion
    }
}
