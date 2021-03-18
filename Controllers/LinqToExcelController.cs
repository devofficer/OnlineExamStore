using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using LinqToExcel;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using System.IO;
using System.Text;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace OnlineExam.Controllers
{
    [Authorize(Roles = "Admin, StaffAdmin")]
    public class LinqToExcelController : Controller
    {
        // GET: /LinqToExcel/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UpdateQuestionImageId()
        {
            return View();
        }
        public class QuestionData
        {
            public int QuestionId { get; set; }
        }
        [HttpPost]
        public ActionResult UpdateQuestionImageId(string s)
        {
            var list = new List<QuestionData>();
            DirectoryInfo dir = new DirectoryInfo(@"D:\CBT\QuestionImages");
            foreach (FileInfo flInfo in dir.GetFiles())
            {
                var name = flInfo.Name;
                string[] parts = flInfo.Name.Split('.');
                var q = Convert.ToString(parts[0].Remove(0, 1));
                if (!string.IsNullOrWhiteSpace(q.Trim()))
                {
                    list.Add(new QuestionData { QuestionId = Convert.ToInt32(q) });
                }
            }

            var dbCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            foreach (var item in list)
            {
                var query = "IF EXISTS (SELECT QuestionId FROM QuestionBank (NOLOCK) WHERE QuestionId = " + item.QuestionId + ")BEGIN UPDATE QuestionBank SET ImagePath = " + item.QuestionId + " WHERE QuestionId = " + item.QuestionId + " END";
                var count = dbCon.Execute(query);
            }

            //using (var dbContext = new ApplicationDbContext())
            //{
            //    using (var dbTran = dbContext.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var notFoundList = new List<QuestionData>();
            //            foreach (var item in list)
            //            {
            //                var questionObj = dbContext.QuestionBank.FirstOrDefault(x => x.QuestionId == item.QuestionId);
            //                if (questionObj != null)
            //                {
            //                    questionObj.ImagePath = item.QuestionId.ToString();
            //                    dbContext.Entry(questionObj).State = EntityState.Modified;
            //                }
            //                else
            //                {
            //                    notFoundList.Add(item);
            //                }
            //            }

            //            dbContext.SaveChanges();
            //            dbTran.Commit();
            //            ViewBag.NotFoundList = String.Join(", ", notFoundList.Select(x => x.QuestionId));
            //        }
            //        catch (Exception ex)
            //        {
            //            dbTran.Rollback();
            //        }
            //    }
            //}


            return View();
        }

        [HttpGet]
        public ActionResult UploadExcel()
        {
            return View(new UploadExcelViewModel());
        }

        [HttpPost]
        public ActionResult UploadExcel(UploadExcelViewModel uploadExcelObj)
        {
            var result = new UploadExcelViewModel();
            result.FileName = "";
            string filePath = "";

            if (!ModelState.IsValid)
            {
                return View(uploadExcelObj);
            }
            try
            {
                byte[] uploadFile = new byte[uploadExcelObj.File.InputStream.Length];
                uploadExcelObj.File.InputStream.Read(uploadFile, 0, uploadFile.Length);
                uploadExcelObj.FileName = uploadExcelObj.File.FileName;
                if (uploadExcelObj.File != null && uploadExcelObj.File.ContentLength > 0)
                {
                    // extract only the fielname
                    //var fileName = Path.GetFileName(uploadExcelObj.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), uploadExcelObj.FileName);
                    DeleteFile(filePath);
                    uploadExcelObj.File.SaveAs(filePath);
                }

                if (ModelState.IsValid)
                {
                    string strExtension = uploadExcelObj.FileName.Split('.')[1].ToLower();
                    if (strExtension == "xlsx" || strExtension == "xls")
                    {
                        if (uploadExcelObj.Format1 || uploadExcelObj.Format2 || uploadExcelObj.Format3 || uploadExcelObj.TopicFormat)
                        {
                            var excelFile = new ExcelQueryFactory(filePath);
                            #region Mapping Excel Cloumn to Model


                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Category, "Category");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Subject, "Subject");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.FormatType, "FormatType");

                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.PassageInstructionID, "Passage/InstructionId");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.PassageText, "PassageText");

                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.QuestionID, "QuestionId");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.QuestionText, "QuestionText");

                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.AnswerOptionA, "OptionA");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.AnswerOptionB, "OptionB");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.AnswerOptionC, "OptionC");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.AnswerOptionD, "OptionD");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.AnswerOptionE, "OptionE");

                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.CorrectAnswer, "CorrectAnswer");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Explanation, "Explanation");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Marks, "Mark");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Time, "Time");
                            excelFile.AddMapping<ExcelFormatViewModel>(x => x.Topic, "Topic");

                            #endregion Mapping Excel Cloumn to Model
                            #region Iterations
                            var worksheetNames = excelFile.GetWorksheetNames();
                            foreach (var sheet in worksheetNames)
                            {
                                if ("topic" == Convert.ToString(sheet) && uploadExcelObj.TopicFormat)
                                {
                                    #region TOPIC
                                    var sheetData = from x in excelFile.Worksheet<ExcelFormatViewModel>(sheet) select x;
                                    var questions = sheetData.ToList<ExcelFormatViewModel>();
                                    uploadExcelObj.InCompleteQuestions = ValidateTopicFormat(questions);
                                    if (uploadExcelObj.InCompleteQuestions != null && uploadExcelObj.InCompleteQuestions.Count > 0)
                                    {
                                        uploadExcelObj.SuccessMsg = "";
                                        uploadExcelObj.ErrorMsg = "Please correct below question errors.";
                                        return View(uploadExcelObj);
                                    }
                                    else
                                    {
                                        UploadTopics(questions, result);
                                        if (uploadExcelObj.InCompleteQuestions != null &&
                                            uploadExcelObj.InCompleteQuestions.Count > 0)
                                        {
                                            return View(uploadExcelObj);
                                        }
                                    }
                                    #endregion

                                }
                                if ("format1" == Convert.ToString(sheet) && uploadExcelObj.Format1)
                                {
                                    #region FORMAT 1
                                    var sheetData = from x in excelFile.Worksheet<ExcelFormatViewModel>(sheet) select x;
                                    var questions = sheetData.ToList<ExcelFormatViewModel>();
                                    uploadExcelObj.InCompleteQuestions = ValidateFormat1And2(questions);
                                    if (uploadExcelObj.InCompleteQuestions != null && uploadExcelObj.InCompleteQuestions.Count > 0)
                                    {
                                        uploadExcelObj.SuccessMsg = "";
                                        uploadExcelObj.ErrorMsg = "Please correct below question errors.";
                                        return View(uploadExcelObj);
                                    }
                                    else
                                    {
                                        UploadQuestions(questions, result);
                                        if (uploadExcelObj.InCompleteQuestions != null &&
                                            uploadExcelObj.InCompleteQuestions.Count > 0)
                                        {
                                            return View(uploadExcelObj);
                                        }
                                    }
                                    #endregion

                                }
                                if ("format2" == Convert.ToString(sheet) && uploadExcelObj.Format2)
                                {
                                    #region FORMAT 2
                                    var sheetData = from x in excelFile.Worksheet<ExcelFormatViewModel>(sheet) select x;
                                    var questions = sheetData.ToList<ExcelFormatViewModel>();
                                    uploadExcelObj.InCompleteQuestions = ValidateFormat1And2(questions);
                                    if (uploadExcelObj.InCompleteQuestions != null && uploadExcelObj.InCompleteQuestions.Count > 0)
                                    {
                                        uploadExcelObj.SuccessMsg = "";
                                        uploadExcelObj.ErrorMsg = "Please correct below question errors.";
                                        return View(uploadExcelObj);
                                    }
                                    else
                                    {
                                        UploadQuestions(questions, result);
                                    }
                                    #endregion
                                }
                                else if ("format3" == Convert.ToString(sheet) && uploadExcelObj.Format3)
                                {
                                    #region FORMAT 3
                                    var sheetData = from x in excelFile.Worksheet<ExcelFormatViewModel>(sheet) select x;
                                    var questions = sheetData.ToList<ExcelFormatViewModel>();
                                    uploadExcelObj.InCompleteQuestions = ValidateFormat3(questions);
                                    if (uploadExcelObj.InCompleteQuestions != null && uploadExcelObj.InCompleteQuestions.Count > 0)
                                    {
                                        uploadExcelObj.SuccessMsg = "";
                                        uploadExcelObj.ErrorMsg = "Please correct below question errors.";
                                        return View(uploadExcelObj);
                                    }
                                    else
                                    {
                                        UploadComprehensiveQuestions(questions, result);
                                    }
                                    #endregion
                                }
                            }
                            DeleteFile(filePath);
                            #endregion
                        }
                        else
                        {
                            result.SuccessMsg = "";
                            result.ErrorMsg = "Please select atleast one format to upload.";
                        }
                    }
                    else
                    {
                        result.SuccessMsg = "";
                        result.ErrorMsg = "Please Upload excel file only.";
                    }
                }
            }
            catch (Exception ex)
            {
                DeleteFile(filePath);
                result.SuccessMsg = "";
                result.ErrorMsg = ex.Message;
            }
            return View(result);
        }

        public FileResult DownloadExcel()
        {
            var filePath = Path.Combine(Server.MapPath("~/App_Data/DownloadFiles"), "QuestionsTemplate.xlsx");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "QuestionsTemplate.xlsx");
        }
        public FileResult DownloadFile()
        {
            string fileName = "QuestionsTemplate.xlsx";
            var filepath = System.IO.Path.Combine(Server.MapPath("~/App_Data/DownloadFiles/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

        private static List<ExcelFormatViewModel> ValidateTopicFormat(List<ExcelFormatViewModel> questions)
        {
            return questions.Where(x => string.IsNullOrWhiteSpace(x.Topic)
                || string.IsNullOrWhiteSpace(x.Subject)
                || x.QuestionID == 0).ToList();
        }
        private static List<ExcelFormatViewModel> ValidateFormat1And2(List<ExcelFormatViewModel> questions)
        {
            return questions.Where(x => string.IsNullOrWhiteSpace(x.QuestionText)
                || string.IsNullOrWhiteSpace(x.AnswerOptionA)
                || string.IsNullOrWhiteSpace(x.AnswerOptionB)
                || string.IsNullOrWhiteSpace(x.AnswerOptionC)
                || string.IsNullOrWhiteSpace(x.AnswerOptionD)
                || string.IsNullOrWhiteSpace(x.Category)
                || string.IsNullOrWhiteSpace(x.Subject)
                || string.IsNullOrWhiteSpace(x.FormatType)
                || string.IsNullOrWhiteSpace(x.CorrectAnswer)
                || x.Marks == 0
                || x.Time == 0
                || x.QuestionID == 0).ToList();
        }
        private static List<ExcelFormatViewModel> ValidateFormat3(List<ExcelFormatViewModel> questions)
        {
            var questionsList = new List<ExcelFormatViewModel>();
            var childQuestions = questions.Where(q => string.IsNullOrWhiteSpace(q.PassageText)).ToList().Where(x => string.IsNullOrWhiteSpace(x.QuestionText)
                || string.IsNullOrWhiteSpace(x.AnswerOptionA)
                || string.IsNullOrWhiteSpace(x.AnswerOptionB)
                || string.IsNullOrWhiteSpace(x.AnswerOptionC)
                || string.IsNullOrWhiteSpace(x.AnswerOptionD)
                || string.IsNullOrWhiteSpace(x.Category)
                || string.IsNullOrWhiteSpace(x.Subject)
                || string.IsNullOrWhiteSpace(x.FormatType)
                || string.IsNullOrWhiteSpace(x.CorrectAnswer)
                || x.Marks == 0
                || x.Time == 0
                || x.QuestionID == 0).ToList();

            questionsList.AddRange(childQuestions);
            var parentQuestions = questions.DistinctBy(x => x.PassageInstructionID).Where(x => string.IsNullOrWhiteSpace(x.PassageText)
                                    || string.IsNullOrWhiteSpace(x.Category)
                                    || string.IsNullOrWhiteSpace(x.Subject)
                                    || string.IsNullOrWhiteSpace(x.FormatType)
                                    || x.Marks == 0
                                    || x.Time == 0
                                    || x.QuestionID == 0).ToList();

            questionsList.AddRange(parentQuestions);
            return questionsList;
        }
        private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        public class SearchClass
        {
            public int QuestionId { get; set; }
        }

        public class SubjectClass
        {
            public int SubjectId { get; set; }
            public string Subject { get; set; }
            public int QuestionId { get; set; }
            public string Topic { get; set; }
        }
        private static void UploadTopics(List<ExcelFormatViewModel> allRecordObj, UploadExcelViewModel result)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                using (var dbTran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var existingQuestions = new List<QuestionBank>();
                        var ids = string.Join(",", allRecordObj.Select(i => i.QuestionID).ToArray());
                        string query = "SELECT * FROM QuestionBank WHERE QuestionId IN (" + ids + ")";
                        try
                        {
                            existingQuestions = dbContext.Database.SqlQuery<QuestionBank>(query).ToList();
                            var existingItems = allRecordObj.Where(i => existingQuestions.Any(ii => ii.QuestionId == i.QuestionID)).ToList();
                            result.InCompleteQuestions.AddRange(existingItems);
                            //if (result.InCompleteQuestions.Count > 0) return;
                        }
                        catch (Exception ex)
                        {
                            result.SuccessMsg = null;
                            result.ErrorMsg = ex.Message;
                            return;
                        }

                        // CHECK IF DUPLICATE QUESTIONS FOUND?
                        if (result.InCompleteQuestions.Count == 0)
                        {
                            //dbContext.SaveChanges();
                            //dbTran.Commit();
                            //result.ErrorMsg = null;
                            //result.SuccessMsg = "Data Uploaded Successfully";
                        }
                        else
                        {
                            var subjects = allRecordObj.Select(e => new SubjectClass { Subject = e.Subject, QuestionId = e.QuestionID, Topic = e.Topic }).DistinctBy(x => x.Subject).ToList();
                            var subjectList = (from s in dbContext.Subjects.ToList()
                                               join ss in subjects on s.Code equals ss.Subject
                                               select new SubjectClass { SubjectId = s.Id, Subject = s.Code, QuestionId = ss.QuestionId, Topic = ss.Topic }).AsEnumerable();

                            var topicId = 0;
                            foreach (var item in subjectList)
                            {
                                var subjectObj = subjectList.FirstOrDefault(e => e.Subject == item.Subject);
                                if (subjectObj != null)
                                {
                                    var topicObj = dbContext.Topics.FirstOrDefault(x => x.Name == item.Topic);
                                    if (topicObj == null)
                                    {
                                        var topic = new Topic
                                        {
                                            Name = item.Topic,
                                            SubjectId = subjectObj.SubjectId,
                                            IsActive = true
                                        };
                                        dbContext.Topics.Add(topic);
                                        dbContext.SaveChanges();
                                        topicId = topic.Id; // LAST INSERTED ID
                                    }
                                    else
                                    {
                                        topicId = topicObj.Id;
                                    }
                                }

                                // QUESTION BANK TABLE WITH TOPIC ID

                                var questionObj = existingQuestions.FirstOrDefault(e => e.QuestionId == item.QuestionId);
                                if (questionObj != null)
                                {
                                    questionObj.TopicId = topicId;
                                    questionObj.ModifiedOn = DateTime.UtcNow;
                                    questionObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                    dbContext.Entry(questionObj).State = EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                            dbTran.Commit();
                            result.ErrorMsg = null;
                            result.SuccessMsg = "Data Uploaded Successfully";
                        }

                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        result.SuccessMsg = null;
                        result.ErrorMsg = ex.Message;
                    }
                }
            }
        }
        private static void UploadQuestions(List<ExcelFormatViewModel> allRecordObj, UploadExcelViewModel result)
        {

            //var duplicateQuestions = new List<ExcelFormatViewModel>();
            using (var dbContext = new ApplicationDbContext())
            {
                using (var dbTran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var ids = string.Join(",", allRecordObj.Select(i => i.QuestionID).ToArray());
                        string query = "SELECT QuestionId FROM QuestionBank WHERE QuestionId IN (" + ids + ")";
                        try
                        {
                            var items = dbContext.Database.SqlQuery<SearchClass>(query).ToList();
                            var existingItems = allRecordObj.Where(i => items.Any(ii => ii.QuestionId == i.QuestionID)).ToList();
                            result.InCompleteQuestions.AddRange(existingItems);
                            if (result.InCompleteQuestions.Count > 0) return;
                        }
                        catch (Exception ex)
                        {
                            result.SuccessMsg = null;
                            result.ErrorMsg = ex.Message;
                            return;
                        }

                        var subjects = allRecordObj.Select(e => new SubjectClass { Subject = e.Subject, QuestionId = e.QuestionID, Topic = e.Topic }).DistinctBy(x => x.Subject).ToList();
                        var subjectList = (from s in dbContext.Subjects.ToList()
                                           join ss in subjects on s.Code equals ss.Subject
                                           select new SubjectClass { SubjectId = s.Id, Subject = s.Code, QuestionId = ss.QuestionId, Topic = ss.Topic }).AsEnumerable();

                        var topics = dbContext.Topics.ToList();
                        foreach (var item in allRecordObj)
                        {
                            var topicId = 0;
                            var subjectObj = subjectList.FirstOrDefault(e => e.Subject == item.Subject);
                            if (subjectObj != null)
                            {
                                var topicObj = topics.FirstOrDefault(x => x.Name == item.Topic);//dbContext.Topics.FirstOrDefault(x => x.Name == item.Topic);
                                if (topicObj == null)
                                {
                                    try
                                    {
                                        var topic = new Topic
                                        {
                                            Name = item.Topic,
                                            SubjectId = subjectObj.SubjectId,
                                            IsActive = true
                                        };
                                        dbContext.Topics.Add(topic);
                                        dbContext.SaveChanges();
                                        topicId = topic.Id; // LAST INSERTED ID
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    topicId = topicObj.Id;
                                }
                            }
                            //result.InCompleteQuestions.FirstOrDefault(x => x.QuestionID == item.QuestionID);//
                            var questionObj = dbContext.QuestionBank.FirstOrDefault(x => x.QuestionId == item.QuestionID);
                            if (questionObj != null)// Question is already exists
                            {
                                #region QUESTION IS ALREADY EXISTS
                                questionObj.Subject = item.Subject;
                                questionObj.Decription = item.QuestionText;
                                questionObj.OptionA = item.AnswerOptionA;
                                questionObj.OptionB = item.AnswerOptionB;
                                questionObj.OptionC = item.AnswerOptionC;
                                questionObj.OptionD = item.AnswerOptionD;
                                questionObj.OptionE = item.AnswerOptionE;
                                questionObj.QuestionFormat = item.FormatType;
                                questionObj.AnswerOption = item.CorrectAnswer;
                                questionObj.AnswerDescription = item.Explanation;
                                questionObj.ExamName = item.Category;
                                questionObj.Mark = item.Marks;
                                questionObj.DurationInSecond = item.Time;
                                questionObj.ImagePath = item.ImagePath;
                                questionObj.IsOnline = true;
                                questionObj.TopicId = topicId;
                                questionObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                questionObj.ModifiedOn = DateTime.Now;
                                dbContext.Entry(questionObj).State = System.Data.Entity.EntityState.Modified;
                                #endregion
                            }
                            else
                            {
                                #region NEW QUESTION
                                var question = new QuestionBank
                                                    {
                                                        // ClassName = item.Class,
                                                        QuestionId = item.QuestionID,
                                                        Subject = item.Subject,
                                                        Decription = item.QuestionText,
                                                        OptionA = item.AnswerOptionA,
                                                        OptionB = item.AnswerOptionB,
                                                        OptionC = item.AnswerOptionC,
                                                        OptionD = item.AnswerOptionD,
                                                        OptionE = item.AnswerOptionE,
                                                        QuestionFormat = item.FormatType,
                                                        AnswerOption = item.CorrectAnswer,
                                                        AnswerDescription = item.Explanation,
                                                        ExamName = item.Category,
                                                        Mark = item.Marks,
                                                        DurationInSecond = item.Time,
                                                        ImagePath = item.ImagePath,
                                                        IsOnline = true,
                                                        CreatedOn = DateTime.Now,
                                                        CreatedBy = CustomClaimsPrincipal.Current.UserId,
                                                        TopicId = topicId
                                                    };
                                dbContext.QuestionBank.Add(question);
                                #endregion
                            }

                            dbContext.SaveChanges();
                        }
                        // CHECK IF DUPLICATE QUESTIONS FOUND?
                        if (result.InCompleteQuestions.Count == 0)
                        {
                            dbTran.Commit();
                            result.ErrorMsg = null;
                            result.SuccessMsg = "Data Uploaded Successfully";
                        }
                        else
                        {
                            result.ErrorMsg = null;
                            result.SuccessMsg = "Found duplicate questions";
                        }

                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        result.SuccessMsg = null;
                        result.ErrorMsg = ex.Message;
                    }
                }
            }
        }

        private static void UploadComprehensiveQuestions(List<ExcelFormatViewModel> allRecordObj, UploadExcelViewModel result)
        {
            IEnumerable<int> parentIds = (from row in allRecordObj
                                          where row.PassageInstructionID > 0
                                          select row.PassageInstructionID).ToList().Distinct();

            var questions = new List<QuestionBank>();
            var dbContext = new ApplicationDbContext();
            using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var a in parentIds)
                    {
                        int? parentQuestionId = null;
                        var childRecords = allRecordObj.Where(x => x.PassageInstructionID == a).ToList();

                        if (childRecords.Count > 1)
                        {
                            var parentRecord = childRecords.Select(x => new QuestionBank
                            {
                                //  ClassName = x.Class,---
                                QuestionId = x.QuestionID,
                                Subject = x.Subject,
                                OptionA = "NA",
                                OptionB = "NA",
                                OptionC = "NA",
                                OptionD = "NA",
                                AnswerOption = "NA",
                                AnswerDescription = "NA",
                                Mark = x.Marks,
                                DurationInSecond = x.Time,
                                Decription = x.PassageText,
                                QuestionFormat = x.FormatType,
                                ExamName = x.Category,
                                ImagePath = "",
                                IsOnline = true,
                                CreatedOn = DateTime.Now,
                                CreatedBy = CustomClaimsPrincipal.Current.UserId,
                            }).FirstOrDefault();

                            if (parentRecord != null)
                            {
                                var questionObj = dbContext.QuestionBank.FirstOrDefault(x => x.QuestionId == parentRecord.QuestionId);
                                if (questionObj != null)// Question is already exists
                                {
                                    questionObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                    questionObj.ModifiedOn = DateTime.Now;
                                    dbContext.Entry(questionObj).State = System.Data.Entity.EntityState.Modified;
                                    parentQuestionId = questionObj.Id;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    dbContext.QuestionBank.Add(parentRecord);
                                    dbContext.SaveChanges();
                                    parentQuestionId = parentRecord.Id;
                                }
                            }
                        }


                        foreach (var item in childRecords)
                        {
                            var questionObj = dbContext.QuestionBank.FirstOrDefault(x => x.QuestionId == item.QuestionID);
                            if (questionObj != null)// Question is already exists
                            {
                                questionObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                questionObj.ModifiedOn = DateTime.Now;
                                dbContext.Entry(questionObj).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                if (parentQuestionId > 0)
                                {
                                    var question = new QuestionBank
                                    {
                                        //ClassName = item.Class,
                                        QuestionId = item.QuestionID,
                                        Subject = item.Subject,
                                        Decription = item.QuestionText,
                                        OptionA = item.AnswerOptionA,
                                        OptionB = item.AnswerOptionB,
                                        OptionC = item.AnswerOptionC,
                                        OptionD = item.AnswerOptionD,
                                        OptionE = item.AnswerOptionE,
                                        QuestionFormat = item.FormatType,
                                        AnswerOption = item.CorrectAnswer,
                                        AnswerDescription = item.Explanation,
                                        ExamName = item.Category,
                                        ImagePath = item.ImagePath,
                                        Mark = item.Marks,
                                        DurationInSecond = item.Time,
                                        IsOnline = true,
                                        CreatedOn = DateTime.Now,
                                        CreatedBy = CustomClaimsPrincipal.Current.UserId,
                                        ParentId = parentQuestionId
                                    };
                                    dbContext.QuestionBank.Add(question);
                                }
                            }
                        }
                        dbContext.SaveChanges();
                    }
                    dbTran.Commit();
                    result.ErrorMsg = null;
                    result.SuccessMsg = "Data Uploaded Successfully";
                }
                catch (Exception ee)
                {
                    dbTran.Rollback();
                    result.SuccessMsg = null;
                    result.ErrorMsg = ee.Message;
                }
            }
        }
    }
}