using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using AutoMapper;
using System.Linq.Expressions;
using LinqKit;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using System.Data.SqlClient;
using System.Data;

namespace OnlineExam.Repositories
{
    public class QuestionBankRepository
    {
        public static IList<QuestionBankViewModel> GetAll()
        {
            List<QuestionBankViewModel> questionBankViewModel = new List<QuestionBankViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                questionBankViewModel = dbContext.Database
                    .SqlQuery<QuestionBankViewModel>("GetQuestions_SP")
                    .ToList();
            }
            return questionBankViewModel;
        }

        public static IList<QuestionBankViewModel> GetQuestionsById(int questionId)
        {
            List<QuestionBankViewModel> questionBankViewModel = new List<QuestionBankViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                var QuestionId = new SqlParameter("@QuestionId", questionId == null ? 0 : questionId);
                questionBankViewModel = dbContext.Database
                    .SqlQuery<QuestionBankViewModel>("GetQuestionsById_SP @QuestionId", QuestionId)
                    .ToList();
            }
            return questionBankViewModel;
        }

        public static IList<QuestionBankViewModel> GetAll(string className, string examName, string subject, string questionFormat, int? mark, bool? isOnline,
           string userId, bool isApplicationUser)
        {
            List<QuestionBankViewModel> questionBankViewModel = new List<QuestionBankViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                var ExamName = new SqlParameter("@Examname", string.IsNullOrEmpty(examName) ? DBNull.Value.ToString() : examName);
                var Subject = new SqlParameter("@Subject", string.IsNullOrEmpty(subject) ? DBNull.Value.ToString() : subject);
                var QuestionFormat = new SqlParameter("@QuestionFormat", string.IsNullOrEmpty(questionFormat) ? DBNull.Value.ToString() : questionFormat);
                var Marks = new SqlParameter("@Marks", mark ?? SqlInt32.Null);
                var IsOnline = new SqlParameter("@IsOnline", isOnline ?? SqlBoolean.Null);

                questionBankViewModel = dbContext.Database
                    .SqlQuery<QuestionBankViewModel>("GetQuestionsByParameters_SP @ExamName,@Subject,@QuestionFormat,@Marks,@IsOnline", ExamName, Subject,QuestionFormat, Marks, IsOnline)
                    .ToList();
            }
            return questionBankViewModel;
        }

        public static IList<RandomQuestionViewModel> GetRandomQuestions(string className, string examName, string subject, string topicIds, string questionFormat, int? mark, bool? isOnline, int noOfRecord)
        {
            List<RandomQuestionViewModel> randomQuestionViewModel = new List<RandomQuestionViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                var ExamName = new SqlParameter("@Examname", string.IsNullOrEmpty(examName) ? DBNull.Value.ToString() : examName);
                var Subject = new SqlParameter("@Subject", string.IsNullOrEmpty(subject) ? DBNull.Value.ToString() : subject);
                var TopicIds = new SqlParameter("@TopicIds", string.IsNullOrEmpty(topicIds) ? DBNull.Value.ToString() : topicIds);
                var QuestionFormat = new SqlParameter("@QuestionFormat", string.IsNullOrEmpty(questionFormat) ? DBNull.Value.ToString() : questionFormat);
                var Marks = new SqlParameter("@Marks", mark ?? SqlInt32.Null);
                var IsOnline = new SqlParameter("@IsOnline", isOnline ?? SqlBoolean.Null);
                var NoOfRecord = new SqlParameter("@NoOfRecords", noOfRecord);

                randomQuestionViewModel = dbContext.Database
                    .SqlQuery<RandomQuestionViewModel>("GetRandomQuestionsByParameters_SP @ExamName,@Subject,@TopicIds,@QuestionFormat,@Marks,@IsOnline, @NoOfRecords", ExamName, Subject,TopicIds, QuestionFormat, Marks, IsOnline, NoOfRecord)
                    .ToList();
            }
            return randomQuestionViewModel;

        }
        public static List<QuestionListViewModel> GetQuestions(string className, string examName, string subject, string topicIds, string questionFormat, int? mark, bool? isOnline,
          string userId, bool isApplicationUser)
        {
            var questionListViewModel = new List<QuestionListViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                var ExamName = new SqlParameter("@Examname", string.IsNullOrEmpty(examName) ? DBNull.Value.ToString() : examName);
                var Subject = new SqlParameter("@Subject", string.IsNullOrEmpty(subject) ? DBNull.Value.ToString() : subject);
                var TopicIds = new SqlParameter("@TopicIds", string.IsNullOrEmpty(topicIds) ? DBNull.Value.ToString() : topicIds);
                var QuestionFormat = new SqlParameter("@QuestionFormat", string.IsNullOrEmpty(questionFormat) ? DBNull.Value.ToString() : questionFormat);
                var Marks = new SqlParameter("@Marks", mark ?? SqlInt32.Null);
                var IsOnline = new SqlParameter("@IsOnline", isOnline?? SqlBoolean.Null);

                questionListViewModel = dbContext.Database
                    .SqlQuery<QuestionListViewModel>("GetQuestionListViewByParameters_SPV2 @ExamName,@Subject,@TopicIds,@QuestionFormat,@Marks,@IsOnline", ExamName, Subject, TopicIds, QuestionFormat, Marks, IsOnline)
                    .ToList();
            }
            return questionListViewModel;
        }
        public static IQueryable<QuestionBankViewModel> SearchByParameters(Expression<Func<QuestionBank, bool>> search)
        {
            var dbContext = new ApplicationDbContext();
            var exams = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.ExamType.ToString() && x.IsActive);
            var subject = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.Subject.ToString() && x.IsActive);
            var questionFormat = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.QuestionFormatType.ToString() && x.IsActive);

            var qry = (from q in dbContext.QuestionBank.Where(search)
                       select new QuestionBankViewModel()
                       {
                           Id = q.Id,
                           Decription = q.Decription,
                           ImagePath = q.ImagePath,
                           OptionA = q.OptionA,
                           OptionB = q.OptionB,
                           OptionC = q.OptionC,
                           OptionD = q.OptionD,
                           OptionE = q.OptionE,
                           AnswerDescription = q.AnswerDescription,
                           IsActive = q.IsActive,
                           CreatedOn = q.CreatedOn,
                           CreatedBy = q.CreatedBy,
                           ModifiedOn = q.ModifiedOn,
                           ModifiedBy = q.ModifiedBy,
                           AnswerOption = q.AnswerOption,
                           // ClassName = classes.FirstOrDefault(x => x.Value == q.ClassName).Text,
                           ExamName = exams.FirstOrDefault(x => x.Value == q.ExamName).Text,
                           Subject = subject.FirstOrDefault(x => x.Value == q.Subject).Text,
                           QuestionFormat = questionFormat.FirstOrDefault(x => x.Value == q.QuestionFormat).Text
                       }).AsQueryable();
            return qry;

        }
    }
}