using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        // GET: Lessons
        public ActionResult Index(int? page)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var dbContext = new ApplicationDbContext();

            var info = dbContext.Lessons.Where(x => x.IsActive==true).ToList()
                .Select(x => new LessonsViewModel
                {
                    LessonId = x.LessonId,
                    LessonGuId = x.LessonGuId,
                    ProfileId = x.ProfileId,
                    TeacherName = dbContext.UserProfiles.Find(x.ProfileId).FullName,
                    ClassType = x.ClassType,
                    SubjectCategory = x.SubjectCategory,
                    Title = x.Title,
                    Description = x.Description,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                    CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    PaymentType = x.PaymentType,
                    Amount = x.Amount,
                    IsActive = x.IsActive == true ? "Yes" : "No",
                    ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy"),
                    IsEnroll = dbContext.LessonUsers.Where(t=>t.LessonId == x.LessonId && t.IsActive==true).Count()>0? "Yes":"No"
                });

            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser)
            {
                string[] classTypes = dbContext.UserProfiles.Find(profileId).ClassTypes.Split('|');
                info = info.Where(x => classTypes.Contains(x.ClassType));
            }

                info = info.OrderByDescending(x => x.StartDate);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = CustomClaimsPrincipal.Current.IsACDAStoreUser;

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult LessonEnroll(int id)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var dbContext = new ApplicationDbContext();
            var info = new LessonUsers();
            info.LessonId = id;
            info.AttendUserProfileId = profileId;
            info.AttendDate = DateTime.Now;
            info.ExpiredDate = DateTime.Now.AddDays(365);
            info.ModifiedDate = DateTime.Now;
            info.IsActive = true;

            dbContext.LessonUsers.Add(info);
            dbContext.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult CreateNote()
        {
            var classCategory = CommonRepository.GetCurrentTeacherClassTypesList();
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

            var subjectCategory = CommonRepository.GetCurrentTeacherSubjectCategoryList();
            ViewBag.subjectCategory = new SelectList(subjectCategory, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNote(LessonNotes model)
        {
            if (!string.IsNullOrEmpty(model.ClassType) && !string.IsNullOrEmpty(model.Subject))
            {
                var dbContext = new ApplicationDbContext();
                string currentUserId = User.Identity.GetUserId();
                int currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;

                model.ProfileId = currentUserProfileId;
                model.CreatedDate = DateTime.Now;
                dbContext.LessonNotes.Add(model);
                dbContext.SaveChanges();
                return RedirectToAction("CreateNote").WithSuccess(AppConstants.SuccessMessageText);
            }
            else
            {
                var classCategory = CommonRepository.GetCurrentTeacherClassTypesList();
                ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

                var subjectCategory = CommonRepository.GetCurrentTeacherSubjectCategoryList();
                ViewBag.subjectCategory = new SelectList(subjectCategory, "Value", "Text");

                return RedirectToAction("CreateNote").WithError("Class category and subject must be select.");
            }
           
        }


        [HttpGet]
        public JsonResult GetClassCategoryByClassType(string classtype)
        {
            if (string.IsNullOrWhiteSpace(classtype))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.ClassCategory.ToString(), classtype);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubjectCategoryByClass(string classcategory)
        {
            if (string.IsNullOrWhiteSpace(classcategory))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString(), classcategory);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTopicBySubjectCategory(string subjectcategory)
        {
            if (string.IsNullOrWhiteSpace(subjectcategory))
                return Json(HttpNotFound());

            var subjects = CommonRepository.GetLookups(Enums.LookupType.Topic.ToString(), subjectcategory);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLesson()
        {
            var classCategory = CommonRepository.GetCurrentTeacherClassTypesList();
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

            var subjectCategory = CommonRepository.GetCurrentTeacherSubjectCategoryList();
            ViewBag.subjectCategory = new SelectList(subjectCategory, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateLesson(Lessons model)
        {
            if (!string.IsNullOrEmpty(model.ClassType) && !string.IsNullOrEmpty(model.SubjectCategory))
            {
                var dbContext = new ApplicationDbContext();
                string currentUserId = User.Identity.GetUserId();
                int currentUserProfileId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;

                model.LessonGuId = Guid.NewGuid();
                model.ProfileId = currentUserProfileId;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                
                dbContext.Lessons.Add(model);
                dbContext.SaveChanges();

                return RedirectToAction("CreateLesson").WithSuccess(AppConstants.SuccessMessageText);
               // return View().WithSuccess(AppConstants.SuccessMessageText);
            }
            else
            {
                var classCategory = CommonRepository.GetCurrentTeacherClassTypesList();
                ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

                var subjectCategory = CommonRepository.GetCurrentTeacherSubjectCategoryList();
                ViewBag.subjectCategory = new SelectList(subjectCategory, "Value", "Text");

                return RedirectToAction("CreateLesson").WithError("Class category and subject category must be select.");
            }
        }


        public ActionResult LessonList(int? page, string sortOrder)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var dbContext = new ApplicationDbContext();

            var info = dbContext.Lessons.Where(x => x.ProfileId == profileId).ToList()
                .Select(x => new LessonsList
                {
                    LessonId = x.LessonId,
                    LessonGuId = x.LessonGuId,
                    ProfileId=x.ProfileId,
                    ClassType = x.ClassType,
                    SubjectCategory = x.SubjectCategory,
                    Title=x.Title,
                    Description=x.Description,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    EndDate=x.EndDate.ToString("dd/MM/yyyy"),
                    CreatedDate=x.CreatedDate.ToString("dd/MM/yyyy"),
                    PaymentType=x.PaymentType,
                    Amount=x.Amount,
                    IsActive=x.IsActive==true?"Yes":"No",
                    ModifiedDate=x.ModifiedDate.ToString("dd/MM/yyyy")
                });

            info = info.OrderByDescending(x => x.StartDate);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateLessonItem(int id, int? page)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.LessonItems.Where(x => x.LessonId == id).ToList()
                .Select(x => new LessonItemsViewModel
                {
                    Id=x.Id,
                    LessonId = x.LessonId,
                    ItemTitle = x.ItemTitle,
                    ItemDescription = x.ItemDescription,
                    FileNames = x.FileNames,
                    DownloadLinks = GetDownloadLinks(x.Id),
                    CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    IsActive = x.IsActive == true ? "Yes" : "No",
                    ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy")
                });

            int pageSize = 20;

            int pageNumber = (page ?? 1);
            ViewBag.ItemsList = info.ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult LessonMaterials(Guid id, int? page)
        {
            var dbContext = new ApplicationDbContext();
            var lessonInfo = dbContext.Lessons.FirstOrDefault(x => x.LessonGuId == id);

            int profileId = lessonInfo.ProfileId;
            int currentUserProfileId = CommonRepository.GetCurrentUserProfileId();

            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser && profileId != currentUserProfileId)
            {
                if (dbContext.LessonUsers.Where(t => t.AttendUserProfileId == currentUserProfileId && t.IsActive == true).Count() == 0)
                    return HttpNotFound();
            }

            ViewBag.LessonTitle = lessonInfo.Title;
            ViewBag.LessonGuid = lessonInfo.LessonGuId;
            var info = dbContext.LessonItems.Where(x => x.LessonId == lessonInfo.LessonId).ToList()
                .Select(x => new LessonItemsViewModel
                {
                    Id = x.Id,
                    LessonId = x.LessonId,
                    ItemTitle = x.ItemTitle,
                    ItemDescription = x.ItemDescription,
                    FileNames = x.FileNames,
                    DownloadLinks = GetDownloadLinks(x.Id),
                    CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    IsActive = x.IsActive == true ? "Yes" : "No",
                    ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy")
                });

            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public static string GetDownloadLinks(long id)
        {
            string downloadLinks = "";
            var dbContext = new ApplicationDbContext();
            var info = dbContext.LessonItems.Find(id);
            string strPath = info.FilePath;
            string strFile = info.FileNames;
            string[] strFiles = strFile.Split(',');

            for(int i=0; i< strFiles.Length; i++)
            {
                if(downloadLinks == "")
                    downloadLinks = "<a href='/Lessons/Download/" + id.ToString() + "/?strfile=" + strFiles[i] + "' title='Download " + strFiles[i] + "' class='btn btn-info btn-xs'>" + strFiles[i] + "</a>";
                else
                    downloadLinks += "&nbsp;&nbsp;<a href='/Lessons/Download/" + id.ToString() + "/?strfile=" + strFiles[i] + "' title='Download " + strFiles[i] + "' class='btn btn-info btn-xs'>" + strFiles[i] + "</a>";
            }
            

            return downloadLinks;
        }

        public ActionResult DownLoad(long id, string strfile)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.LessonItems.Find(id);
            string strPath = Path.Combine(Server.MapPath(info.FilePath + "/"), strfile); ;
            byte[] fileBytes = System.IO.File.ReadAllBytes(strPath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strfile);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateLessonItem(int id, LessonItems model, HttpPostedFileBase[] files)
        {
            var dbContext = new ApplicationDbContext();
            string lessonGuid = dbContext.Lessons.Find(id).LessonGuId.ToString();
            string strFileNames = "";
            string folderId = Guid.NewGuid().ToString();
            string filePath = "~/FileUploads/Lessons/" + lessonGuid + "/" + folderId;
  
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var type = Path.GetExtension(file.FileName);
                    // You have to make sure ‘FileUploads’ directory exists 
                    // string savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "FileUploads/", fileName);
                    if (file != null)
                    {
                        CreateIfMissing(filePath);
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);

                        var fullPath = Path.Combine(Server.MapPath(filePath + "/"), fileName);

                        file.SaveAs(fullPath);

                        if (strFileNames == "")
                            strFileNames = fileName;
                        else
                            strFileNames +="," + fileName;
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.ItemTitle))
            {
                model.LessonId = id;
                model.FolderName = folderId;
                model.FilePath = filePath;
                model.FileNames = strFileNames;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;

                dbContext.LessonItems.Add(model);
                dbContext.SaveChanges();

                return RedirectToAction("CreateLessonItem").WithSuccess(AppConstants.SuccessMessageText);
                // return View().WithSuccess(AppConstants.SuccessMessageText);
            }
            else
            {
                return RedirectToAction("CreateLessonItem").WithError("Error.. Try Again.");
            }


        }

        public ActionResult DeleteLessonItem(long id)
        {
            var dbContext = new ApplicationDbContext();
            
            var info = dbContext.LessonItems.Find(id);
            int lessonId = info.LessonId;
            string lessonGuid = dbContext.Lessons.Find(lessonId).LessonGuId.ToString();
            string folderPath = Server.MapPath("~/FileUploads/Lessons/" + lessonGuid + "/" + info.FolderName);
            DeleteDirectory(folderPath);

            dbContext.LessonItems.Remove(info);
            dbContext.SaveChanges();

            return RedirectToAction("CreateLessonItem",new {id= lessonId }).WithSuccess("Record has been deleted");
        }

        public void DeleteDirectory(string path)
        {
            // Delete all files from the Directory  
            foreach (string filename in Directory.GetFiles(path))
            {
                System.IO.File.Delete(filename);
            }
            // Check all child Directories and delete files  
            foreach (string subfolder in Directory.GetDirectories(path))
            {
                DeleteDirectory(subfolder);
            }
            Directory.Delete(path);
        }

        private string CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(Server.MapPath(path));
            if (!folderExists)
                Directory.CreateDirectory(Server.MapPath(path));
            return path;
        }


        public ActionResult LessonDiscussion(Guid id, int? page)
        {
            var dbContext = new ApplicationDbContext();
            var lessonInfo = dbContext.Lessons.FirstOrDefault(x => x.LessonGuId == id);
            int profileId = lessonInfo.ProfileId;
            int currentUserProfileId = CommonRepository.GetCurrentUserProfileId();

            if(!CustomClaimsPrincipal.Current.IsACDAStoreUser && profileId != currentUserProfileId)
            {
                if(dbContext.LessonUsers.Where(t=>t.AttendUserProfileId == currentUserProfileId && t.IsActive == true).Count() == 0)
                    return HttpNotFound();
            }


            ViewBag.LessonTitle = lessonInfo.Title;
            ViewBag.LessonDescription = lessonInfo.Description;
            ViewBag.ClassType = lessonInfo.ClassType;
            ViewBag.Subject = lessonInfo.SubjectCategory;
            
            var teacherInfo = dbContext.UserProfiles.Find(profileId);
            ViewBag.Teacher = teacherInfo.FullName;
            ViewBag.LessonGuid = id;

            var info = dbContext.LessonDiscussions.Where(x=>x.LessonId == lessonInfo.LessonId && x.IsActive==true)
                       .ToList()
                       .Select(x=> new LessonDiscussionsViewModel()
                       {
                           Id = x.Id,
                           LessonId = x.LessonId,
                           ProfileId = x.ProfileId,
                           UserName = dbContext.UserProfiles.Find(x.ProfileId).FirstName,
                           UserAvater = dbContext.UserProfiles.Find(x.ProfileId).Avatar,
                           Note = x.Note,
                           CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                           ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy HH:mm"),
                           IsActive = x.IsActive == true ? "Yes" : "No"
                       });

                

            int pageSize = 20;

            int pageNumber = (page ?? 1);

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreatePost(Guid id, FormCollection f)
        {
            var dbContext = new ApplicationDbContext();
            var lessonInfo = dbContext.Lessons.FirstOrDefault(x => x.LessonGuId == id);
            int lessonId = lessonInfo.LessonId;
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var strNote = f["txtPost"];
            if (!string.IsNullOrEmpty(strNote))
            {
                var info = new LessonDiscussions();
                info.LessonId = lessonId;
                info.ProfileId = profileId;
                info.Note = strNote;
                info.CreatedDate = DateTime.Now;
                info.ModifiedDate = DateTime.Now;
                info.IsActive = true;

                dbContext.LessonDiscussions.Add(info);
                dbContext.SaveChanges();
            }

            return RedirectToAction("LessonDiscussion",new {id=id });
        }


        [HttpPost]
        public JsonResult CKEditorFileUpload(HttpPostedFileWrapper upload, string CKEditor, string CKEditorFuncNum, string langCode)
        {
            string fileName = "";
            if (upload.ContentLength > 0)
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + upload.FileName;
                string path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["FileUploadPath"]), fileName);

                // var stream = new FileStream(path, FileMode.Create);
                upload.SaveAs(path);
            }

            return Json(new { Path = "/Upload/" + fileName }, JsonRequestBehavior.AllowGet);
           // return Json("test", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CKEditorFileBrowser()
        {
            string path = ConfigurationManager.AppSettings["FileUploadPath"];
            ViewBag.Path = path;

            return View();
        }

        public ActionResult ImageViewer(string directory)
        {
            string path = ConfigurationManager.AppSettings["FileUploadPath"];
            ViewBag.directory = path;
            if (directory != "Upload" && !string.IsNullOrEmpty(directory))
            {
                ViewBag.directory = path + "/" + directory;
            }


            return View();
        }

        public FileResult GenerateThumb(string image)
        {
            if (string.IsNullOrEmpty(image))
            {
                return null;
            }
            string image1 = AppContext.BaseDirectory + image;
            var thumbWidth = 128D;
            byte[] buffer = GlobalUtilities.GenerateTumbnail(image1, thumbWidth);


            return File(buffer, "image/jpeg");
        }

        public string DisplayLink(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }
            string link = AppContext.BaseDirectory + file;

            return link;
        }


    }
}