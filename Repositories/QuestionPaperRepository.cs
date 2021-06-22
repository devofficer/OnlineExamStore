using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LinqKit;
using Microsoft.Ajax.Utilities;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace OnlineExam.Repositories
{
    public class QuestionPaperRepository
    {
        public static IList<QuestionPaperViewModel> GetAll(string className, string examName, string subject, string questionFormat, int? mark,
            bool? isOnline)
        {
            var dbContext = new ApplicationDbContext();
            var predicate = PredicateBuilder.True<QuestionBank>();

            //if (!string.IsNullOrWhiteSpace(className))
            //{
            //    predicate = predicate.And(x => x.ClassName.Contains(className));
            //}
            if (!string.IsNullOrWhiteSpace(examName))
            {
                predicate = predicate.And(x => x.ExamName.Contains(examName));
            }
            if (!string.IsNullOrWhiteSpace(subject))
            {
                predicate = predicate.And(x => x.Subject.Contains(subject));
            }
            if (!string.IsNullOrWhiteSpace(questionFormat))
            {
                predicate = predicate.And(x => x.QuestionFormat.Contains(questionFormat));
            }
            if (!string.IsNullOrWhiteSpace(Convert.ToString(mark)))
            {
                predicate = predicate.And(x => x.Mark == mark);
            }
            if (!string.IsNullOrWhiteSpace(Convert.ToString(isOnline)))
            {
                predicate = predicate.And(x => x.IsOnline == isOnline);
            }

            var questionFormats = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.QuestionFormatType.ToString() && x.IsActive);
            //var query = dbContext.QuestionBank.AsExpandable().Where(predicate) as IOrderedQueryable<QuestionBank>;
            var qry = (from qb in dbContext.QuestionBank.AsExpandable().Where(predicate)
                       where qb.ParentId == 0 || qb.ParentId == null
                       select qb).AsEnumerable().Select((item, index) => new QuestionPaperViewModel
                       {
                           Index = index + 1,
                           Id = item.Id,
                           Decription = item.Decription,
                           QuestionFormat = questionFormats.FirstOrDefault(x => x.Value == item.QuestionFormat) != null ? questionFormats.FirstOrDefault(x => x.Value == item.QuestionFormat).Text : item.QuestionFormat
                       }).ToList();
            return qry;
        }
        public class QuestionPaperData
        {

            public int Index { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Minute { get; set; }
            public bool IsTrial { get; set; }
            public bool IsOnline { get; set; }
            public string ClassName { get; set; }
            public string Subject { get; set; }
            public int TotalQuestions { get; set; }
            public int TotalMarks { get; set; }
            public int TimeTakenInMinutes { get; set; }
            public int TotalObtainedMarks { get; set; }
            public int Status { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public static IList<QuestionPaperListViewModel> GetAll(string className, string examName, string subject, string opType, string userId, string cbtType,string userType)
        {
            var questionPaperListViewModel = new List<QuestionPaperListViewModel>();

            if (userType == "Student")
            {
                string currentUserId = CustomClaimsPrincipal.Current.UserId;
                using (var dbContext = new ApplicationDbContext())
                {
                    var ClassName = new SqlParameter("@ClassName", string.IsNullOrEmpty(className) ? DBNull.Value.ToString() : className);
                    var ExamName = new SqlParameter("@Examname", string.IsNullOrEmpty(examName) ? DBNull.Value.ToString() : examName);
                    var Subject = new SqlParameter("@Subject", string.IsNullOrEmpty(subject) ? DBNull.Value.ToString() : subject);
                    var UserId = new SqlParameter("@UserId", string.IsNullOrEmpty(currentUserId) ? DBNull.Value.ToString() : currentUserId);
                    var TeacherId = new SqlParameter("@TeacherId", string.IsNullOrEmpty(userId) ? DBNull.Value.ToString() : userId);
                    var OpType = new SqlParameter("@OpType", string.IsNullOrEmpty(opType) ? DBNull.Value.ToString() : opType);
                    var IsACDAStoreUser = new SqlParameter("@IsACDAStoreUser", CustomClaimsPrincipal.Current.IsACDAStoreUser);
                    var CBTType = new SqlParameter("@CBTType", string.IsNullOrEmpty(cbtType) ? DBNull.Value.ToString() : cbtType);

                    questionPaperListViewModel =
                   dbContext.Database
                    .SqlQuery<QuestionPaperListViewModel>("GetStudentQuestionPaperListViewModel_SP @ClassName, @ExamName, @Subject, @UserId, @TeacherId, @OpType, @CBTType", ClassName, ExamName, Subject, UserId, TeacherId, OpType, CBTType).ToList();
                }
                return questionPaperListViewModel;
            }
            else
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var ClassName = new SqlParameter("@ClassName", string.IsNullOrEmpty(className) ? DBNull.Value.ToString() : className);
                    var ExamName = new SqlParameter("@Examname", string.IsNullOrEmpty(examName) ? DBNull.Value.ToString() : examName);
                    var Subject = new SqlParameter("@Subject", string.IsNullOrEmpty(subject) ? DBNull.Value.ToString() : subject);
                    var UserId = new SqlParameter("@UserId", string.IsNullOrEmpty(userId) ? DBNull.Value.ToString() : userId);
                    var OpType = new SqlParameter("@OpType", string.IsNullOrEmpty(opType) ? DBNull.Value.ToString() : opType);
                    var IsACDAStoreUser = new SqlParameter("@IsACDAStoreUser", CustomClaimsPrincipal.Current.IsACDAStoreUser);
                    var CBTType = new SqlParameter("@CBTType", string.IsNullOrEmpty(cbtType) ? DBNull.Value.ToString() : cbtType);

                    questionPaperListViewModel =
                   dbContext.Database
                    .SqlQuery<QuestionPaperListViewModel>("GetQuestionPaperListViewModel_SP @ClassName, @ExamName, @Subject, @UserId, @OpType, @IsACDAStoreUser, @CBTType", ClassName, ExamName, Subject, UserId, OpType, IsACDAStoreUser, CBTType).ToList();
                }

                return questionPaperListViewModel;
            }


            ////var dbContext = new ApplicationDbContext();
            ////var predicate = PredicateBuilder.True<QuestionPaper>();

            ////if (!string.IsNullOrWhiteSpace(className))
            ////{
            ////    predicate = predicate.And(x => x.ClassName.Contains(className));
            ////}
            ////if (!string.IsNullOrWhiteSpace(examName))
            ////{
            ////    predicate = predicate.And(x => x.ExamName.Contains(examName));
            ////}
            ////if (!string.IsNullOrWhiteSpace(subject))
            ////{
            ////    predicate = predicate.And(x => x.Subject.Contains(subject));
            ////}
            ////var classes = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.ClassType.ToString() && x.IsActive);
            ////var exams = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.ExamType.ToString() && x.IsActive);
            ////var subjects = dbContext.Lookup.Where(x => x.ModuleCode == Enums.LookupType.Subject.ToString() && x.IsActive);

            ////if (opType == "#attempted-tab")
            ////{
            ////    #region attempted-tab
            ////    questionPaperListViewModel = (from q in dbContext.QuestionPapers.AsExpandable().Where(predicate)
            ////                                  join aq in dbContext.AttemptedQuestionPapars on q.Id equals aq.QuestionPaparId
            ////                                  where aq.CreatedBy == userId
            ////                                  select new QuestionPaperListViewModel
            ////                                  {
            ////                                      Id = aq.Id,
            ////                                      CreatedBy = aq.CreatedBy,
            ////                                      CreatedOn = aq.CreatedOn,
            ////                                      ModifiedBy = aq.ModifiedBy,
            ////                                      ModifiedOn = aq.ModifiedOn,
            ////                                      TotalQuestions = aq.TotalQuestions,
            ////                                      TotalMarks = aq.TotalMarks,
            ////                                      TotalObtainedMarks = aq.TotalObtainedMarks,
            ////                                      TimeTakenInMinutes = aq.TimeTakenInMinutes,
            ////                                      Name = q.Name,
            ////                                      Minute = q.Minute,
            ////                                      IsTrial = q.IsTrial,
            ////                                      IsOnline = q.IsOnline,
            ////                                      ClassName = q.ClassName,
            ////                                      ExamName = q.ExamName,
            ////                                      Subject = q.Subject
            ////                                  }).OrderByDescending(x => x.CreatedOn)
            ////                .ThenByDescending(x => x.Name)
            ////                .ThenByDescending(x => x.ClassName)
            ////                .ThenByDescending(x => x.ExamName)
            ////                .ThenByDescending(x => x.Subject).ToList();
            ////    #endregion
            ////}
            ////else
            ////{
            ////    if (!CustomClaimsPrincipal.Current.IsACDAStoreUser)
            ////    {
            ////        questionPaperListViewModel = (from q in dbContext.QuestionPapers.AsExpandable().Where(predicate)
            ////                                      join aq in dbContext.AttemptedQuestionPapars.Where(x => x.CreatedBy == userId)
            ////                                          on q.Id equals aq.QuestionPaparId
            ////                                          into a
            ////                                      from c in a.DefaultIfEmpty()
            ////                                      select new QuestionPaperListViewModel
            ////                                      {
            ////                                          Id = q.Id,
            ////                                          IsActive = q.IsActive,
            ////                                          CreatedBy = q.CreatedBy,
            ////                                          CreatedOn = q.CreatedOn,
            ////                                          ModifiedBy = q.ModifiedBy,
            ////                                          ModifiedOn = q.ModifiedOn,
            ////                                          Name = q.Name,
            ////                                          Minute = q.Minute,
            ////                                          IsTrial = q.IsTrial,
            ////                                          IsOnline = q.IsOnline,
            ////                                          ClassName = q.ClassName,
            ////                                          ExamName = q.ExamName,
            ////                                          Subject = q.Subject,
            ////                                          TotalQuestions = q.QuestionPaperMappings.Count,
            ////                                          //TotalMarks = q.QuestionPaperMappings,
            ////                                          Status = c == null ? 1 : (c != null && (DbFunctions.DiffDays(c.CreatedOn, DateTime.Now) > 5)) ? 2 : 3
            ////                                      }).OrderByDescending(x => x.CreatedOn)
            ////            .ThenByDescending(x => x.Name)
            ////            .ThenByDescending(x => x.ClassName)
            ////            .ThenByDescending(x => x.ExamName)
            ////            .ThenByDescending(x => x.Subject).DistinctBy(x => x.Id).ToList();

            ////        questionPaperListViewModel.ForEach(x =>
            ////        {
            ////            var questionListObj = dbContext.QuestionPaperMappings.Include("QuestionBank").Where(q => q.QuestionPaperId == x.Id);
            ////            if (questionListObj != null)
            ////            {
            ////                x.TotalMarks = questionListObj.Sum(e => e.QuestionBank.Mark);
            ////            }
            ////        });
            ////    }
            ////    else
            ////    {
            ////        questionPaperListViewModel = (from qb in dbContext.QuestionPapers
            ////            .OrderByDescending(x => x.CreatedOn)
            ////            .ThenByDescending(x => x.Name)
            ////            .ThenByDescending(x => x.ClassName)
            ////            .ThenByDescending(x => x.ExamName)
            ////            .ThenByDescending(x => x.Subject)
            ////            .AsExpandable().Where(predicate)
            ////                                      select qb).AsEnumerable().Select((item, index) => new QuestionPaperListViewModel
            ////            {
            ////                Index = index + 1,
            ////                Id = item.Id,
            ////                IsActive = item.IsActive,
            ////                CreatedBy = item.CreatedBy,
            ////                CreatedOn = item.CreatedOn,
            ////                ModifiedBy = item.ModifiedBy,
            ////                ModifiedOn = item.ModifiedOn,
            ////                Name = item.Name,
            ////                Minute = item.Minute,
            ////                IsTrial = item.IsTrial,
            ////                IsOnline = item.IsOnline,
            ////                ClassName = item.ClassName,
            ////                ExamName = item.ExamName,
            ////                Subject = item.Subject
            ////            }).ToList();
            ////        questionPaperListViewModel.ForEach(x =>
            ////        {
            ////            var questionListObj = dbContext.QuestionPaperMappings.Include("QuestionBank").Where(q => q.QuestionPaperId == x.Id).ToList();
            ////            if (questionListObj != null && questionListObj.Any())
            ////            {
            ////                x.TotalQuestions = questionListObj.Count();
            ////                x.TotalMarks = questionListObj.Sum(e => e.QuestionBank.Mark);
            ////            }
            ////        });
            ////    }
            ////}

            ////questionPaperListViewModel.ForEach(x =>
            ////{
            ////    var classObj = classes.FirstOrDefault(c => c.Value == x.ClassName);
            ////    if (classObj != null)
            ////        x.ClassName = classObj.Text;
            ////    var examObj = exams.FirstOrDefault(c => c.Value == x.ExamName);
            ////    if (examObj != null)
            ////        x.ExamName = examObj.Text;
            ////    var subjectObj = subjects.FirstOrDefault(c => c.Value == x.Subject);
            ////    if (subjectObj != null)
            ////        x.Subject = subjectObj.Text;
            ////    x.Minute = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToInt32(x.Minute)).TotalMinutes);
            ////    x.TimeTakenInMinutes = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToInt32(x.TimeTakenInMinutes)).TotalMinutes);
            ////});
            ////return questionPaperListViewModel;
        }

        public static IList<QuestionPaperViewModel> GetAll(int questionPaperId)
        {
            var dbContext = new ApplicationDbContext();
            var questionPaperObj = dbContext.QuestionPapers.FirstOrDefault(x => x.Id == questionPaperId);
            if (questionPaperObj != null)
            {
                var qry = (from qp in questionPaperObj.QuestionPaperMappings
                           join qb in dbContext.QuestionBank on qp.QuestionBankId equals qb.Id
                           select qb).AsEnumerable().Select((item, index) => new QuestionPaperViewModel
                          {
                              Index = index + 1,
                              Id = item.Id,
                              Decription = item.Decription,
                              QuestionFormat = item.QuestionFormat
                          }).ToList();
                return qry;
            }
            return new List<QuestionPaperViewModel>();
        }
    }
}