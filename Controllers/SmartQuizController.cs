using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OnlineExam.Helpers;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Web;
using OnlineExam.Infrastructure;

namespace OnlineExam.Controllers
{
    public class TAnswerResult
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public int AnswerQuestionId { get; set; }
        public int ParentId { get; set; }
        public string AnswerOption { get; set; }
        public string Answer { get; set; }
        public string AnswerStatus { get; set; }
        public int Marks { get; set; }

        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string AnswerDescription { get; set; }
    }
    public class TResult
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationInMinutes { get; set; }
        public int TimeTakenInSecond { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttemptedQuestions { get; set; }
        public int TotalCorrectedAnswers { get; set; }
        public int TotalInCorrectedAnswers { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public bool IsCompletelyAttempted { get; set; }
        public string FinalStatus { get; set; }
        public string UserId { get; set; }
    }

    public class TopScorerViewModel
    {
        public string Name { get; set; }
        public int Marks { get; set; }
        public int TimeTakenInSecond { get; set; }
        public string UserId { get; set; }
    }

    public class QuestionSummaryViewModel
    {
        public TResult Summary { get; set; }
        public List<TAnswerResult> ListOfTAnswerResult { get; set; }

        public List<TopScorerViewModel> TopScorerViewModel { get; set; }
    }
    [AuthorizeUser()]
    public class SmartQuizController : Controller
    {
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                //REDIRECT TO NOT FOUND VIEW
                return View("QuestionNotFound");
            }
            return View();
        }
        public ActionResult IndexV2()
        {
            return View();
        }
        public ActionResult Demo(int? id)
        {
            if (id == null)
            {
                //REDIRECT TO NOT FOUND VIEW
                //return View("QuestionNotFound");
            }
            return View();
        }
        public ActionResult Summary(int? id)
        {
            if (id == null)
            {
                //REDIRECT TO NOT FOUND VIEW
                return View("QuestionNotFound");
            }
            var questionPaperViewModel = new List<QuestionPaperViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                int userMaxId = 1;
                var f = dbContext.AnswerQuestions.OrderByDescending(r => r.CreatedOn).FirstOrDefault(
                    x => x.UserId == CustomClaimsPrincipal.Current.UserId && x.QuestionPaperId == id
                    && x.ParentId == null);
                if (f != null)
                {
                    userMaxId = f.UserMaxId;
                }
                return View(LoadQuestionPaperResultSet(Convert.ToInt32(id), userMaxId));
            }
        }

        private QuestionSummaryViewModel LoadQuestionPaperResultSet(int questionPaperId, int userMaxId)
        {
            var questionSummaryViewModel = new QuestionSummaryViewModel();
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
                    var p6 = new SqlParameter { ParameterName = "@UserMaxId", SqlDbType = SqlDbType.Int, Value = userMaxId };

                    db.Database.Connection.Open();
                    dbCommand.Parameters.Add(p1);
                    dbCommand.Parameters.Add(p2);
                    dbCommand.Parameters.Add(p3);
                    dbCommand.Parameters.Add(p4);
                    dbCommand.Parameters.Add(p5);
                    dbCommand.Parameters.Add(p6);

                    using (var reader = dbCommand.ExecuteReader())
                    {
                        questionSummaryViewModel.Summary = reader.MapToList<TResult>().FirstOrDefault();
                        reader.NextResult();
                        questionSummaryViewModel.ListOfTAnswerResult = reader.MapToList<TAnswerResult>();
                        reader.NextResult();
                        questionSummaryViewModel.TopScorerViewModel = reader.MapToList<TopScorerViewModel>();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.Connection.Close();
                }

                return questionSummaryViewModel;
            }
        }
    }
}
