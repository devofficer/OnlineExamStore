using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    public class SchoolController : Controller
    {
        IUnitOfWork unitOfWork = new UnitOfWork();
        public SchoolController()
        {

        }
        [HttpGet]
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    int Id = Convert.ToInt32(id);
                    var getSchoolById = contextObj.Schools.Find(Id);
                    if (getSchoolById != null && getSchoolById.IsActive && getSchoolById.Logo != null && getSchoolById.Logo.Contains(".."))
                    {
                        getSchoolById.Logo = getSchoolById.Logo.Replace("..", "");
                    }
                    return View("Index", "_SchoolLayout", getSchoolById == null ? new School() : getSchoolById);
                }
            }
            else
                return View(new School());
        }
        [AuthorizeUser(Roles = "Admin, StaffAdmin, SchoolAdmin")]
        public JsonResult GetAll()
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var schools = contextObj.Schools.Where(x => x.IsActive).ToList().OrderByDescending(x => x.Id);
                return Json(schools, JsonRequestBehavior.AllowGet);
            }
        }
        [AuthorizeUser(Roles = "Admin, StaffAdmin, SchoolAdmin")]
        public JsonResult GetBannersById(int? schoolId)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var banners = contextObj.Banners.Where(x => x.SchoolId == schoolId).ToList().OrderByDescending(x => x.Id);
                return Json(banners, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LoadBanners(int? schoolId)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var banners = contextObj.Banners.Where(x => x.SchoolId == schoolId && x.IsActive).ToList().OrderByDescending(x => x.Id);
                return Json(banners, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSchoolById(string id)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                int Id = Convert.ToInt32(id);
                var getSchoolById = contextObj.Schools.Find(Id);
                return Json(getSchoolById.IsActive ? getSchoolById : new School(), JsonRequestBehavior.AllowGet);
            }
        }

        public string UpdateSchool(School school)
        {
            if (school != null)
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    int empId = Convert.ToInt32(school.Id);
                    School schoolObj = contextObj.Schools.Where(a => a.Id == empId && a.IsActive).FirstOrDefault();
                    schoolObj.Name = school.Name;

                    schoolObj.Name = school.Name;
                    schoolObj.Logo = school.Logo;
                    schoolObj.Description = school.Description;
                    schoolObj.Url = school.Url;
                    schoolObj.TemplateName = school.TemplateName;
                    schoolObj.Address = school.Address;
                    schoolObj.ContactName = school.ContactName;
                    schoolObj.ContactPhone = school.ContactPhone;
                    schoolObj.City = school.City;
                    schoolObj.State = school.State;
                    schoolObj.Status = school.Status;
                    //employee.d = emp.Email;
                    //employee.Age = emp.Age;
                    contextObj.SaveChanges();
                    return "School Updated";
                }
            }
            else
            {
                return "Invalid Record";
            }
        }

        public JsonResult GetAllNewsById(int? schoolId)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var news = contextObj.SchoolNews.Where(x => x.SchoolId == schoolId).ToList().OrderByDescending(x => x.Id);
                return Json(news, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LoadNewsById(int schoolId)
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var news = contextObj.SchoolNews.Where(x => x.SchoolId == schoolId && x.IsActive).ToList().OrderByDescending(x => x.Id);
                return Json(news, JsonRequestBehavior.AllowGet);
            }
        }

        public string AddNews(SchoolNews schoolNews)
        {
            try
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    schoolNews.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                    schoolNews.CreatedOn = DateTime.UtcNow;
                    contextObj.SchoolNews.Add(schoolNews);
                    contextObj.SaveChanges();
                    return AppConstants.RecordSavedText;
                }
            }
            catch (Exception ex)
            {
                return AppConstants.ErrorMessageText;
            }
        }

        public ActionResult Directory()
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var schools = contextObj.Schools.Where(x => x.IsActive).ToList().OrderByDescending(x => x.Name).ToList();
                return View(schools);
            }
        }

        [HttpPost]
        public JsonResult SaveFiles(int? employeeId)
        {
            var message = "Done";
            var status = false;
            var SavedfileName = "";
            string schoolCode = "000";
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var actualFileName = file.FileName;
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                int size = file.ContentLength;

                try
                {
                    using (ApplicationDbContext contextObj = new ApplicationDbContext())
                    {
                        var schoolObj = contextObj.Schools.FirstOrDefault(x => x.Id == employeeId);

                        // EDIT SCHOOL LOGO
                        if (schoolObj != null && schoolObj.Logo != null)
                        {
                            schoolCode = schoolObj.Code;
                            string ExistingFilePath = Path.Combine(Server.MapPath("~/FileUploads/School/Logos/" + schoolCode + "/"), schoolObj.Logo.Split('/')[5]);
                            if ((System.IO.File.Exists(ExistingFilePath)))
                            {
                                System.IO.File.Delete(ExistingFilePath);
                            }
                            else
                            {
                                schoolCode = GlobalUtilities.GetStringRandomNumber(4);
                                CreateSchoolDirectories(schoolCode);

                            }
                            SavedfileName = "../FileUploads/School/Logos/" + schoolCode + "/" + fileName;
                            file.SaveAs(Path.Combine(Server.MapPath("~/FileUploads/School/Logos/" + schoolCode + "/"), fileName));
                        }
                        else
                        {
                            schoolCode = GlobalUtilities.GetStringRandomNumber(4);
                            CreateSchoolDirectories(schoolCode);
                            SavedfileName = "../FileUploads/School/Logos/" + schoolCode + "/" + fileName;
                            file.SaveAs(Path.Combine(Server.MapPath("~/FileUploads/School/Logos/" + schoolCode + "/"), fileName));
                        }
                    }

                    status = true;
                }
                catch (Exception ex)
                {
                    message = "File upload failed! Please try again";
                }
            }
            return new JsonResult { Data = new { Message = message, Status = status, SchoolCode = schoolCode, FileName = SavedfileName } };
        }


        public string AddSchool(School school)
        {
            if (ModelState.IsValid)
            {
                if (school != null)
                {
                    using (ApplicationDbContext contextObj = new ApplicationDbContext())
                    {
                        if (!CustomClaimsPrincipal.Current.IsACDAStoreUser)
                            school.Status = "Created";
                        school.IsActive = true;
                        contextObj.Schools.Add(school);
                        contextObj.SaveChanges();

                        return "School Added";
                    }
                }
                else
                {
                    return "Invalid Record";
                }
            }
            else
            {
                return "Invalid Record";
            }
        }

        private void CreateSchoolDirectories(string schoolId)
        {
            bool isLogoExists = System.IO.Directory.Exists(Server.MapPath("~/FileUploads/School/Logos/" + schoolId));

            if (!isLogoExists)
                System.IO.Directory.CreateDirectory(Server.MapPath("~/FileUploads/School/Logos/" + schoolId));

            bool isEventExists = System.IO.Directory.Exists(Server.MapPath("~/FileUploads/School/Events/" + schoolId));

            if (!isEventExists)
                System.IO.Directory.CreateDirectory(Server.MapPath("~/FileUploads/School/Events/" + schoolId));

            bool isSlideExists = System.IO.Directory.Exists(Server.MapPath("~/FileUploads/School/Slides/" + schoolId));

            if (!isSlideExists)
                System.IO.Directory.CreateDirectory(Server.MapPath("~/FileUploads/School/Slides/" + schoolId));
        }

        public string DeleteEmployee(string employeeId)
        {
            if (!String.IsNullOrEmpty(employeeId))
            {
                try
                {
                    int Id = Convert.ToInt32(employeeId);
                    using (ApplicationDbContext contextObj = new ApplicationDbContext())
                    {
                        var schoolObj = contextObj.Schools.Find(Id);
                        schoolObj.IsActive = false;
                        contextObj.Entry(schoolObj).State = System.Data.Entity.EntityState.Modified;
                        //contextObj.Schools.Remove(schoolObj);
                        contextObj.SaveChanges();
                        return "School Deleted";
                    }
                }
                catch (Exception ex)
                {
                    return "School Not Found";
                }
            }
            else
            {
                return "Invalid Request";
            }
        }

        [HttpPost]
        public string DeleteBannerById(int bannerId)
        {
            try
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    var bannerObj = contextObj.Banners.Find(bannerId);
                    //schoolObj.IsActive = false;
                    if (bannerObj != null)
                    {
                        contextObj.Entry(bannerObj).State = System.Data.Entity.EntityState.Modified;
                        contextObj.Banners.Remove(bannerObj);
                        contextObj.SaveChanges();
                        return "Banner Deleted";
                    }
                    else
                    {
                        return "Banner Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                return AppConstants.ErrorMessageText;
            }
        }

         [HttpPost]
        public string DeleteNewsById(int newsId)
        {
            try
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    var schoolNewsObj = contextObj.SchoolNews.Find(newsId);
                    //schoolObj.IsActive = false;
                    if (schoolNewsObj != null)
                    {
                        contextObj.Entry(schoolNewsObj).State = System.Data.Entity.EntityState.Modified;
                        contextObj.SchoolNews.Remove(schoolNewsObj);
                        contextObj.SaveChanges();
                        return "News Deleted";
                    }
                    else
                    {
                        return "News Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                return AppConstants.ErrorMessageText;
            }
        }        

        [HttpPost]
        public string EditBannerById(Banner banner)
        {
            try
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    var bannerObj = contextObj.Banners.Find(banner.Id);

                    if (bannerObj != null)
                    {
                        bannerObj.IsActive = banner.IsActive;
                        contextObj.Entry(bannerObj).State = System.Data.Entity.EntityState.Modified;
                        contextObj.SaveChanges();
                        return "Banner Edited";
                    }
                    else
                    {
                        return "Banner Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                return AppConstants.ErrorMessageText;
            }
        }

          [HttpPost]
        public string EditNewsById(SchoolNews schoolNews)
        {
            try
            {
                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    var schoolNewsObj = contextObj.SchoolNews.Find(schoolNews.Id);

                    if (schoolNewsObj != null)
                    {
                        schoolNewsObj.IsActive = schoolNews.IsActive;
                        schoolNewsObj.IsNew = schoolNews.IsNew;
                        schoolNewsObj.Description = schoolNews.Description;
                        contextObj.Entry(schoolNewsObj).State = System.Data.Entity.EntityState.Modified;
                        contextObj.SaveChanges();
                        return "News Edited";
                    }
                    else
                    {
                        return "News Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                return AppConstants.ErrorMessageText;
            }
        }

        

        public ActionResult GetLogo()
        {
            return Content("~/images/logo.png");
        }
        public JsonResult FillState()
        {
            using (ApplicationDbContext contextObj = new ApplicationDbContext())
            {
                var states = contextObj.States.ToList().Select(x => new dropdown { id = x.StateCode, Name = x.StateText });
                return Json(states, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FillAction()
        {
            return Json(BindActions(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCity(string StateId)
        {
            using (var contextObj = new ApplicationDbContext())
            {
                var cities = contextObj.Cities.ToList().Where(x => x.StateCode == StateId).Select(x => new dropdown { id = x.CityCode, Name = x.CityText }); ;
                return Json(cities, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UploadBanners(int employeeId)
        {
            var message = "Done";
            var status = false;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var actualFileName = file.FileName;
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                int size = file.ContentLength;

                using (ApplicationDbContext contextObj = new ApplicationDbContext())
                {
                    using (var trans = contextObj.Database.BeginTransaction())
                    {
                        try
                        {
                            if (contextObj.Schools.Any(x => x.Id == employeeId))
                            {
                                // INSERT
                                var schoolCode = contextObj.Schools.FirstOrDefault(x => x.Id == employeeId).Code;
                                if (!string.IsNullOrWhiteSpace(schoolCode))
                                {
                                    //INSERT
                                    string ExistingFilePath = Path.Combine(Server.MapPath("~/FileUploads/School/Slides/" + schoolCode));
                                    if ((System.IO.Directory.Exists(ExistingFilePath)))
                                    {
                                        var banner = new Banner();
                                        banner.SchoolId = employeeId;
                                        //banner.IsActive = true;
                                        banner.Url = "/FileUploads/School/Slides/" + schoolCode + "/" + fileName;
                                        banner.CreatedOn = DateTime.UtcNow;
                                        banner.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                                        contextObj.Banners.Add(banner);

                                        //SavedfileName = "../FileUploads/School/Logos/" + schoolCode + "/" + fileName;
                                        file.SaveAs(Path.Combine(Server.MapPath("~/FileUploads/School/Slides/" + schoolCode + "/"), fileName));

                                        contextObj.SaveChanges();
                                        trans.Commit();
                                        status = true;
                                    }
                                }
                                else
                                {
                                    message = "School record doesn't found! Please try again";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.Commit();
                            message = AppConstants.ErrorMessageText;
                            status = false;
                            return new JsonResult { Data = new { Message = message, Status = status } };
                        }
                    }
                }
            }
            return new JsonResult { Data = new { Message = message, Status = status } };
        }

        public class dropdown
        {
            public string id { get; set; }
            public string Name { get; set; }
        }

        public class City
        {
            public int id { get; set; }
            public int stateId { get; set; }
            public string Name { get; set; }
        }

        public List<City> BindCity()
        {
            List<City> lCity = new List<City>();
            lCity.Add(new City { id = 1, stateId = 1, Name = "fbd" });
            lCity.Add(new City { id = 2, stateId = 1, Name = "palwal" });

            lCity.Add(new City { id = 3, stateId = 2, Name = "Noida" });
            lCity.Add(new City { id = 4, stateId = 2, Name = "Agra" });

            lCity.Add(new City { id = 5, stateId = 3, Name = "Burari" });
            lCity.Add(new City { id = 6, stateId = 3, Name = "jangpura" });


            return lCity;
        }

        public List<dropdown> BindActions()
        {
            List<dropdown> lAction = new List<dropdown>();
            lAction.Add(new dropdown { id = "Created", Name = "Created" });
            lAction.Add(new dropdown { id = "Approved", Name = "Approved" });
            lAction.Add(new dropdown { id = "De-Activate", Name = "De-Activate" });
            return lAction;
        }
    }
}