using Microsoft.AspNet.Identity;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [Authorize]
    public class TeachersController : Controller
    {
        // GET: Teachers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Followers(int? page)
        {
            var dbContext = new ApplicationDbContext();
            int teacherProfileId = CommonRepository.GetCurrentUserProfileId();

            var info = dbContext.Followers.Where(x => x.UserProfileId == teacherProfileId)
                .ToList()
                .Select(x => new TeacherFollowViewModel()
                {
                    Id = x.Id,
                    TeacherId = x.UserProfileId,
                    TeacherName = dbContext.UserProfiles.Find(x.UserProfileId).FullName,
                    FollowUserId = x.FollowersUserProfileId,
                    FollowUserName = dbContext.UserProfiles.Find(x.FollowersUserProfileId).FullName,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    UserType = CommonRepository.GetUserTypeByProfileId(x.FollowersUserProfileId),
                    ClassType = dbContext.UserProfiles.Find(x.FollowersUserProfileId).ClassTypes.Replace("|", ", "),
                    GroupName = x.GroupId.Value>0?dbContext.FollowerGroups.Find(x.GroupId.Value).GroupName:""
                }); ;

            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult CreateGroup(FormCollection f)
        {
            var dbContext = new ApplicationDbContext();
            int teacherProfileId = CommonRepository.GetCurrentUserProfileId();
            string strGrop = f["txtGroup"];

            if (!string.IsNullOrEmpty(strGrop))
            {
                if (dbContext.FollowerGroups.Where(x => x.GroupName == strGrop && x.ProfileId == teacherProfileId).Count() == 0)
                {
                    var info = new FollowerGroups();
                    info.ProfileId = teacherProfileId;
                    info.GroupName = strGrop;
                    info.CreatedDate = DateTime.Now;

                    dbContext.FollowerGroups.Add(info);
                    dbContext.SaveChanges();
                    return RedirectToAction("Followers").WithSuccess("Record has been created");
                }
                else
                {
                    return RedirectToAction("Followers").WithError("Group name already exists");
                }
            }
            else
            {
                return RedirectToAction("Followers").WithError("Group name cannot be empty");
            }
        }

        public ActionResult AssignUserToGroup(int id)
        {
            var dbContext = new ApplicationDbContext();
            int teacherProfileId = CommonRepository.GetCurrentUserProfileId();
            var userInfo = dbContext.UserProfiles.Find(id);
            var info = dbContext.FollowerGroups.Where(x => x.ProfileId == teacherProfileId).OrderBy(x => x.GroupName).ToList();
            ViewBag.GroupList = new SelectList(info, "Id", "GroupName");
            ViewBag.UserId = id;
            ViewBag.UserName = userInfo.FullName;
            ViewBag.ClassType = userInfo.ClassTypes.Replace("|", ", ");

            return View();
        }

        [HttpPost]
        public ActionResult AssignUserToGroup(int id, FormCollection f)
        {
            var dbContext = new ApplicationDbContext();
            int teacherProfileId = CommonRepository.GetCurrentUserProfileId();
            var userInfo = dbContext.UserProfiles.Find(id);

            try
            {
                var info = dbContext.Followers.FirstOrDefault(x => x.FollowersUserProfileId == id && x.UserProfileId == teacherProfileId);
                info.GroupId = int.Parse(f["lstGroup"]);
                dbContext.SaveChanges();

                return RedirectToAction("Followers").WithSuccess("User has been assign to selected group");
            }
            catch
            {
                var info = dbContext.FollowerGroups.Where(x => x.ProfileId == teacherProfileId).OrderBy(x => x.GroupName).ToList();
                ViewBag.GroupList = new SelectList(info, "Id", "GroupName");
                ViewBag.UserId = id;
                ViewBag.UserName = userInfo.FullName;
                ViewBag.ClassType = userInfo.ClassTypes.Replace("|", ", ");
                return View().WithError("Record not saved!");
            }
            
        }

        public ActionResult TeachersList(int? page)
        {
            var dbContext = new ApplicationDbContext();
            int profileId = CommonRepository.GetCurrentUserProfileId();

            string classType = dbContext.UserProfiles.Find(profileId).ClassTypes;

            var info = from t in dbContext.UserProfiles
                       where t.ClassTypes.Contains(classType) && t.ApplicationUser.UserType == "Teacher"
                       select new TeachersViewModel
                       {
                            TeacherId = t.UserProfileId,
                            TeacherName = t.FirstName + " " + t.LastName,
                            ClassType = t.ClassTypes,
                            SubjectType = t.SubjectCategory,
                            IsFollowed = dbContext.Followers.Where(x=>x.UserProfileId == t.UserProfileId && x.FollowersUserProfileId == profileId).Count() > 0?"Yes":"No"
                       };

            info = info.OrderBy(x=>x.TeacherId);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Follow(int id)
        {
            var dbContext = new ApplicationDbContext();
            int profileId = CommonRepository.GetCurrentUserProfileId();

            if(dbContext.Followers.Where(x => x.UserProfileId == id && x.FollowersUserProfileId == profileId).Count() == 0)
            {
                var follow = new Followers();
                follow.UserProfileId = id;
                follow.FollowersUserProfileId = profileId;
                follow.StartDate = DateTime.Now;

                dbContext.Followers.Add(follow);
                dbContext.SaveChanges();
            }

            return RedirectToAction("TeachersList");
        }


        public ActionResult EnrollQuestionPaperList(int? page)
        {
            var dbContext = new ApplicationDbContext();
            int profileId = CommonRepository.GetCurrentUserProfileId();
            string currentUserId = User.Identity.GetUserId();

            string classType = dbContext.UserProfiles.Find(profileId).ClassTypes;

            var info = from t in dbContext.QuestionPapers
                       where t.CreatedBy == currentUserId && t.IsActive==true && t.IsOnline == true
                       from a in dbContext.AttemptedQuestionPapars
                       where a.QuestionPaparId == t.Id
                       select new EnrollQuestionPaperViewModel
                       {
                           AttenId = a.Id,
                           QuestionPaperId = t.Id,
                           AttendUserId = a.UserId,
                           StudentProfileId = dbContext.UserProfiles.FirstOrDefault(x=>x.ApplicationUser.Id == a.UserId).UserProfileId,
                           AttendStudentName = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == a.UserId).FirstName + " " + dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == a.UserId).LastName,
                           ClassType = t.ClassName,
                           CategoryType = t.ExamName,
                           SubjectType = t.Subject,
                           QuestionPaperTitle=t.Name,
                           TimeTaken=a.TimeTakenInMinutes,
                           TotalQuestions=a.TotalQuestions,
                           TotalMarks=a.TotalMarks,
                           TotalObtainedMarks=a.TotalObtainedMarks,
                           TotalCorrectedAnswered=a.TotalCorrectedAnswered,
                           TotalInCorrectedAnswered=a.TotalInCorrectedAnswered,
                           IsCompleted=a.IsCompletelyAttempted==true?"Yes":"No",
                           Status=a.Status,
                           StartDate=a.CreatedOn,
                           ModifyDate = a.ModifiedOn
                       };

            info = info.OrderByDescending(x => x.StartDate);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult QuestionPaperSuggestions(int? page)
        {
            var dbContext = new ApplicationDbContext();
            int profileId = CommonRepository.GetCurrentUserProfileId();
            string currentUserId = User.Identity.GetUserId();

            string classType = dbContext.UserProfiles.Find(profileId).ClassTypes;

            var info = from t in dbContext.QuestionPapers
                       where t.ClassName == classType && t.IsActive == true && t.IsOnline == true
                       from u in dbContext.UserProfiles
                       where t.CreatedBy == u.ApplicationUser.Id
                       from f in dbContext.Followers
                       where f.UserProfileId == u.UserProfileId && f.FollowersUserProfileId == profileId
                       select new QuestionPaperSuggestionViewModel
                       {
                           QuestionPaperId = t.Id,
                           TotalQuestions= dbContext.QuestionPaperMappings.Where(x=>x.QuestionPaperId == t.Id).Count(),
                           TotalTime = t.Minute,
                           TeacherProfileId = u.UserProfileId,
                           TeacherName = u.FirstName + " " + u.LastName,
                           ClassType = t.ClassName,
                           CategoryType = t.ExamName,
                           SubjectType = t.Subject,
                           QuestionPaperTitle = t.Name,
                           CreatedDate = t.CreatedOn,
                           TotalAttend = dbContext.AttemptedQuestionPapars.Where(x=>x.QuestionPaparId == t.Id && x.UserId == currentUserId).Count()
                       };

            info = info.Where(a=>a.TotalAttend == 0).OrderByDescending(x => x.CreatedDate);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "StaffAdmin,SchoolAdmin")]
        public ActionResult QuestionPendingList(int? page, string strClass, string strCategory, string strSubject, string strFormat)
        {
            ViewBag.strClass = strClass;
            ViewBag.strCategory = strCategory;
            ViewBag.strSubject = strSubject;

            var dbContext = new ApplicationDbContext();

            var classes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString());
            var questionFormats = CommonRepository.GetLookups(Enums.LookupType.QuestionFormatType.ToString());

            ViewBag.classTypes = new SelectList(classes, "Text", "Text");
            ViewBag.questionFormats = new SelectList(questionFormats, "Text", "Text", strFormat);

            var info = from t in dbContext.SystemQuestionRequiest
                       where t.IsApproved == false
                       from q in dbContext.QuestionBank
                       where q.QuestionId == t.QuestionId
                       from u in dbContext.UserProfiles where u.UserProfileId == t.TeacherProfileId
                       select new PendinQuestionViewModel
                       {
                           Id = t.Id,
                           TeacherId = t.TeacherProfileId,
                           QuestionId = t.QuestionId,
                           TeacherName = u.FirstName + " " + u.LastName,
                           Description = q.Decription,
                           OptionA = q.OptionA,
                           OptionB = q.OptionB,
                           OptionC = q.OptionC,
                           OptionD = q.OptionD,
                           OptionE = q.OptionE,
                           AnswerOption = q.AnswerOption,
                           AnswerDescription = q.AnswerDescription,
                           QuestionFormat = q.QuestionFormat,
                           CategoryType = q.ExamName,
                           SubjectType = q.Subject,
                           CreatedDate = t.CreatedDate
                       };

            if (!string.IsNullOrEmpty(strCategory))
            {
                info = info.Where(x => x.CategoryType == strCategory);
            }

            if (!string.IsNullOrEmpty(strSubject))
            {
                info = info.Where(x => x.SubjectType == strSubject);
            }

            if (!string.IsNullOrEmpty(strFormat))
            {
                info = info.Where(x => x.QuestionFormat == strFormat);
            }

            info = info.OrderByDescending(x => x.CreatedDate);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "StaffAdmin,SchoolAdmin")]
        [HttpPost]
        public ActionResult SystemQuestionRequestUpdate(FormCollection f)
        {
            string updateType = f["updateType"];
            string ids = f["Ids"];
            string[] IdsArray = ids.Split(',');
            int id = 0;
            if (updateType == "Approve")
            {
                for (int i = 0; i < IdsArray.Length; i++)
                {
                    id = int.Parse(IdsArray[i]);
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var info = dbContext.SystemQuestionRequiest.Find(id);
                        info.IsApproved = true;
                        info.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();

                        int questionId = info.QuestionId;

                        var q = dbContext.QuestionBank.FirstOrDefault(x => x.QuestionId == questionId);
                        q.IsSystem = true;
                        dbContext.SaveChanges();
                    }
                }
            }

            if (updateType == "Reject")
            {
                for (int i = 0; i < IdsArray.Length; i++)
                {
                    id = int.Parse(IdsArray[i]);
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var info = dbContext.SystemQuestionRequiest.Find(id);
                        dbContext.SystemQuestionRequiest.Remove(info);
                        dbContext.SaveChanges();
                    }
                }
            }


                return RedirectToAction("QuestionPendingList");
        }
    }
}