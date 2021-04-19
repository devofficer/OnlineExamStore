using OnlineExam.Helpers;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private ApplicationDbContext _dbContext = new ApplicationDbContext();
        // GET: Messages
        public ActionResult Index(int? page)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var info = _dbContext.MessageDetails.ToList()
                       .Select (m=>new MessageDetailsViewModel() { 
                           Id=m.Id,
                           MessageGuId = m.MessageGuId,
                           AssignUserType = m.AssignUserType,
                           ClassType = m.ClassType,
                           AssignUserId = m.AssignUserId,
                           MessageText = m.MessageText,
                           CreatedBy = m.CreatedBy,
                           UserName = _dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == m.CreatedBy).FullName,
                           UserAvater = _dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == m.CreatedBy).Avatar,
                           CreatedDate = m.CreatedDate.ToString("dd/MM/yyyy H:mm"),
                           ReadUserIds = m.ReadUserIds,
                           ReplyAllowed = m.ReplyAllowed,
                           ReplyCount = _dbContext.MessagesReplies.Where(x=>x.MessageId == m.Id).Count(),
                           IsActive = m.IsActive
                       });

            info = info.Where(x => x.IsActive == true);
            if (User.IsInRole("StaffAdmin"))
            {
                info = info.Where(x => x.CreatedBy == profileId || x.AssignUserType.Contains("StaffAdmin"));
            }
            else
            {
                var currentUserInfo = _dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == profileId);
                string[] strClassTypes = currentUserInfo.ClassTypes.Split('|');
                if (User.IsInRole("Teacher"))
                {
                    info = info.Where(x => x.CreatedBy == profileId ||(x.AssignUserType.Contains("Teacher") && strClassTypes.Contains(x.ClassType)));
                }
                if (User.IsInRole("Student"))
                {
                    info = info.Where(x => x.CreatedBy == profileId || (x.AssignUserType.Contains("Student") && strClassTypes.Contains(x.ClassType)));
                }
            }

            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = CustomClaimsPrincipal.Current.IsACDAStoreUser;
            ViewBag.CurrentUserProfileId = profileId;

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var classCategory = CommonRepository.GetAllClassTypesList();
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

            ViewBag.Groups = "";
            ViewBag.Teachers = "";

            if (User.IsInRole("Teacher"))
            {
                var groupsInfo = _dbContext.FollowerGroups.Where(x => x.ProfileId == profileId).ToList();
                ViewBag.Groups = new SelectList(groupsInfo, "Id", "GroupName");
            }

            if (User.IsInRole("StaffAdmin"))
            {
                var teacherInfo = _dbContext.UserProfiles.Where(x => x.UserProfileId == profileId && x.ApplicationUser.Roles.Any(t => t.RoleId == _dbContext.Roles.FirstOrDefault(r => r.Name == "Teacher").Id)).ToList();
                ViewBag.Teachers = new SelectList(teacherInfo, "UserProfileId", "FullName");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(MessageDetails model)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            model.MessageGuId = Guid.NewGuid();
            model.CreatedBy = profileId;
            model.CreatedDate = DateTime.Now;
            model.IsActive = true;
            _dbContext.MessageDetails.Add(model);
            _dbContext.SaveChanges();

            return RedirectToAction("Index").WithSuccess(AppConstants.SuccessMessageText);
        }

        public ActionResult Delete(Guid id)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var info = _dbContext.MessageDetails.FirstOrDefault(x => x.MessageGuId == id);
            info.IsActive = false;

            _dbContext.SaveChanges();

            return RedirectToAction("Index").WithSuccess("Message has been deleted!");
        }

        public ActionResult Reply(Guid id)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var info = _dbContext.MessageDetails.FirstOrDefault(x => x.MessageGuId == id);
            if(info.ReplyAllowed == false)
            {
                return HttpNotFound();
            }

            ViewBag.Message = info.MessageText;
            ViewBag.MessageGuId = info.MessageGuId;
            ViewBag.MessageId = info.Id;

            return View();
        }

        [HttpPost]
        public ActionResult Reply(Guid id, MessagesReplies model)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            try
            {
                model.ReplyGuId = Guid.NewGuid();
                model.UserId = profileId;
                model.CreatedDate = DateTime.Now;

                _dbContext.MessagesReplies.Add(model);
                _dbContext.SaveChanges();

                return RedirectToAction("Index").WithSuccess("Reply has been recorded!");
            }
            catch
            {
                var info = _dbContext.MessageDetails.FirstOrDefault(x => x.MessageGuId == id);
                if (info.ReplyAllowed == false)
                {
                    return HttpNotFound();
                }

                ViewBag.Message = info.MessageText;
                ViewBag.MessageGuId = info.MessageGuId;
                ViewBag.MessageId = info.Id;

                return View().WithError("Reply not created!");
            }
        }


        public ActionResult ViewReply(int? page, Guid id)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var messageInfo = _dbContext.MessageDetails.FirstOrDefault(x => x.MessageGuId == id);
            long messageId = messageInfo.Id;
            ViewBag.Messages = messageInfo.MessageText;

            var info = _dbContext.MessagesReplies.ToList()
                       .Select(m => new MessagesRepliesViewModel()
                       {
                           Id = m.Id,
                           MessageId = m.MessageId,
                           ReplyGuId = m.ReplyGuId,
                           UserId = m.UserId,
                           ReplyText = m.ReplyText,
                           UserName = _dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == m.UserId).FullName,
                           UserAvater = _dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == m.UserId).Avatar,
                           CreatedDate = m.CreatedDate.ToString("dd/MM/yyyy H:mm")
                       });

            info = info.Where(x=>x.MessageId == messageId).OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = CustomClaimsPrincipal.Current.IsACDAStoreUser;
            ViewBag.CurrentUserProfileId = profileId;

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult GetStudentsByClass(string strClass)
        {
            string[] classArr = strClass.Split('|');
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var info = from p in _dbContext.UserProfiles
                       select p;
            if (strClass != "All")
            {
                info = info.Where(x => classArr.Contains(x.ClassTypes));
            }

            var studentList = new SelectList(info, "UserProfileId", "FullName");

            return Json(studentList, JsonRequestBehavior.AllowGet);
        }

       
    }
}