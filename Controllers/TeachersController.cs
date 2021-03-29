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
    }
}