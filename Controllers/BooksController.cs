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
    public class BooksController : Controller
    {
        // GET: Books
        public ActionResult Index(int? page)
        {
            var dbContext = new ApplicationDbContext();

            var info = dbContext.Books.Where(x => x.IsActive == true).ToList()
                .Select(x => new BooksViewModel
                {
                    Id = x.Id,
                    ClassType = x.ClassType,
                    Title = x.Title,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    IsActive = x.IsActive == true ? "Yes" : "No",
                    ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy")                    
                });

            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = CustomClaimsPrincipal.Current.IsACDAStoreUser;

            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var classCategory = CommonRepository.GetAllClassTypesList();
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Books model)
        {
            var dbContext = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                if (dbContext.Books.Where(x => x.Title == model.Title && x.ClassType == model.ClassType).Count() == 0)
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.IsActive = true;
                    dbContext.Books.Add(model);
                    dbContext.SaveChanges();

                    return RedirectToAction("Create").WithSuccess(AppConstants.SuccessMessageText);
                }
                else
                {
                    var classCategory = CommonRepository.GetAllClassTypesList();
                    ViewBag.classCategory = new SelectList(classCategory, "Value", "Text",model.ClassType);

                    return View().WithError("Book Title already exists!");
                }
            }

            return View();
        }

        public ActionResult Edit(int id)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.Books.Find(id);

            var classCategory = CommonRepository.GetAllClassTypesList();
            ViewBag.classCategory = new SelectList(classCategory, "Value", "Text", info.ClassType);

            return View(info);
        }

        [HttpPost]
        public ActionResult Edit(int id, Books model)
        {
            var dbContext = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                if (dbContext.Books.Where(x => x.Id != id && x.Title == model.Title && x.ClassType == model.ClassType).Count() == 0)
                {
                    var info = dbContext.Books.Find(id);
                    info.ClassType = model.ClassType;
                    info.Title = model.Title;
                    info.Description = model.Description;
                    info.ModifiedDate = DateTime.Now;
                    info.IsActive = true;

                    dbContext.SaveChanges();

                    return RedirectToAction("Index").WithSuccess(AppConstants.SuccessMessageText);
                }
                else
                {
                    var classCategory = CommonRepository.GetAllClassTypesList();
                    ViewBag.classCategory = new SelectList(classCategory, "Value", "Text", model.ClassType);

                    return View().WithError("Book Title already exists!");
                }
            }

            return View();
        }

        public ActionResult Delete(int id)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.Books.Find(id);

            info.ModifiedDate = DateTime.Now;
            info.IsActive = false;

            dbContext.SaveChanges();

            return RedirectToAction("Index").WithSuccess(info.Title + " Book successfully deleted");
        }


        //************Assign Books*************
        public ActionResult AssignBooks(int? page, int? id)
        {
            var dbContext = new ApplicationDbContext();
            int profileId = CommonRepository.GetCurrentUserProfileId();
            ViewBag.IsOwner = "No";
            var info = dbContext.BooksAssign.ToList()
                .Select(x => new BooksAssignViewModel
                {
                    Id = x.Id,
                    AssignGuId = x.AssignGuId,
                    TeacherId = x.TeacherId,
                    TeacherName = dbContext.UserProfiles.Find(profileId).FullName,
                    BookId = x.BookId,
                    ClassType = dbContext.Books.Find(x.BookId).ClassType,
                    BookTitle = dbContext.Books.Find(x.BookId).Title,
                    TimeFrame = x.TimeFrame,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                    dtEndDate = x.EndDate,
                    IsActive = x.IsActive == true ? "Yes" : "No",
                    ModifiedDate = x.ModifiedDate.ToString("dd/MM/yyyy")
                });;

            if (User.IsInRole("Teacher"))
            {
                info = info.Where(x => x.TeacherId == profileId);
                ViewBag.IsOwner = "Yes";
            }
            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = CustomClaimsPrincipal.Current.IsACDAStoreUser;
            ViewBag.BookList = null;

            string userId = CustomClaimsPrincipal.Current.UserId;

            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser)
            {
                string[] classTypes = dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == profileId).ClassTypes.Split('|');

                var booksInfo = dbContext.Books.Where(x => x.IsActive == true && classTypes.Contains(x.ClassType)).ToList();
                ViewBag.BookList = new SelectList(booksInfo, "Id", "Title");
            }
            
            ViewBag.BookTitle = "";
            ViewBag.TimeFrame = "";
            ViewBag.ItemId = 0;
            ViewBag.ProfileId = profileId;

            if (id > 0)
            {
                ViewBag.ItemId = id;
                var info1 = dbContext.BooksAssign.Find(id);
                int BookId = info1.BookId;
                ViewBag.BookId = dbContext.Books.Find(BookId).Id;
                ViewBag.TimeFrame = info1.TimeFrame;
            }

            return View(info.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult AssignBooksCreate(FormCollection f)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            int AssignId = int.Parse(f["Id"]);
            var dbContext = new ApplicationDbContext();
            if (!string.IsNullOrEmpty(f["BookId"]))
            {
                if (AssignId == 0)
                {
                    if (dbContext.BooksAssign.Where(x => x.TeacherId == profileId && x.IsActive==true && x.EndDate >= DateTime.Now).Count() == 0)
                    {
                        var info = new BooksAssign();
                        info.AssignGuId = Guid.NewGuid();
                        info.TeacherId = profileId;
                        info.BookId = int.Parse(f["BookId"]);
                        info.TimeFrame = f["TimeFrame"];
                        info.StartDate = DateTime.Now;

                        if (f["TimeFrame"] == "One Week")
                            info.EndDate = DateTime.Now.AddDays(7);

                        if (f["TimeFrame"] == "Two Weeks")
                            info.EndDate = DateTime.Now.AddDays(14);

                        if (f["TimeFrame"] == "One Month")
                            info.EndDate = DateTime.Now.AddDays(30);

                        info.ModifiedDate = DateTime.Now;
                        info.IsActive = true;

                        dbContext.BooksAssign.Add(info);
                        dbContext.SaveChanges();

                        return RedirectToAction("AssignBooks").WithSuccess("Book successfully assigned!");
                    }
                    else
                    {
                        return RedirectToAction("AssignBooks").WithError("You already have assgin book in current period!");
                    }
                }
                else
                {
                    var info = dbContext.BooksAssign.Find(AssignId);
                    info.BookId = int.Parse(f["BookId"]);
                    info.TimeFrame = f["TimeFrame"];

                    if (f["TimeFrame"] == "One Week")
                        info.EndDate = DateTime.Now.AddDays(7);

                    if (f["TimeFrame"] == "Two Weeks")
                        info.EndDate = DateTime.Now.AddDays(14);

                    if (f["TimeFrame"] == "One Month")
                        info.EndDate = DateTime.Now.AddDays(30);

                    info.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();

                    return RedirectToAction("AssignBooks").WithSuccess("Book assigned successfully updated!");
                }
            }
            else
            {
                return RedirectToAction("AssignBooks").WithError("Books cannot be empty! Please select book");
            }
        }

        //public ActionResult AssignBooksEdit(int id, FormCollection f)
        //{
        //    var dbContext = new ApplicationDbContext();
        //    var info = dbContext.BooksAssign.Find(id);
        //    info.BookId = int.Parse(f["BookId"]);
        //    info.TimeFrame = f["TimeFrame"];

        //    if (f["TimeFrame"] == "One Week")
        //        info.EndDate = DateTime.Now.AddDays(7);

        //    if (f["TimeFrame"] == "Two Week")
        //        info.EndDate = DateTime.Now.AddDays(14);

        //    if (f["TimeFrame"] == "One Month")
        //        info.EndDate = DateTime.Now.AddDays(30);

        //    info.ModifiedDate = DateTime.Now;
        //    dbContext.SaveChanges();

        //    return RedirectToAction("AssignBooks").WithSuccess("Book assigned successfully updated!");
        //}

        public ActionResult AssignBooksDelete(int id)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.BooksAssign.Find(id);

            info.IsActive = false;
            info.ModifiedDate = DateTime.Now;
            dbContext.SaveChanges();

            return RedirectToAction("AssignBooks").WithSuccess("Book assigned successfully deleted!");
        }

        //************Books Review*************

        public ActionResult BooksReview(Guid id, int? page)
        {
            var dbContext = new ApplicationDbContext();
            var assignInfo = dbContext.BooksAssign.FirstOrDefault(x => x.AssignGuId == id);
            var bookInfo = dbContext.Books.Find(assignInfo.BookId);
            int profileId = assignInfo.TeacherId;
            int currentUserProfileId = CommonRepository.GetCurrentUserProfileId();

            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser && profileId != currentUserProfileId)
            {
                if (dbContext.LessonUsers.Where(t => t.AttendUserProfileId == currentUserProfileId && t.IsActive == true).Count() == 0)
                    return HttpNotFound();
            }


            ViewBag.BookTitle = bookInfo.Title;
            ViewBag.BookDescription = bookInfo.Description;
            ViewBag.ClassType = bookInfo.ClassType;


            var teacherInfo = dbContext.UserProfiles.Find(profileId);
            ViewBag.Teacher = teacherInfo.FullName;
            ViewBag.AssignGuid = id;

            ViewBag.IsActive = "Yes";
            ViewBag.CurrentTecherId = assignInfo.TeacherId;
            ViewBag.CurrentUserId = currentUserProfileId;

            if (assignInfo.IsActive == false || assignInfo.EndDate < DateTime.Now)
                ViewBag.IsActive = "No";

            var info = dbContext.BooksReview.Where(x => x.AssignId == assignInfo.Id)
                       .ToList()
                       .Select(x => new BooksReviewViewModel()
                       {
                           Id = x.Id,
                           AssignId = x.AssignId,
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
        public ActionResult CreateReview(Guid id, FormCollection f)
        {
            var dbContext = new ApplicationDbContext();
            var reviewInfo = dbContext.BooksAssign.FirstOrDefault(x => x.AssignGuId == id);
            int assignId = reviewInfo.Id;
            int profileId = CommonRepository.GetCurrentUserProfileId();

            var strNote = f["txtPost"];
            if (!string.IsNullOrEmpty(strNote))
            {
                var info = new BooksReview();
                info.AssignId = assignId;
                info.ProfileId = profileId;
                info.Note = strNote;
                info.CreatedDate = DateTime.Now;
                info.ModifiedDate = DateTime.Now;
                info.IsActive = true;

                dbContext.BooksReview.Add(info);
                dbContext.SaveChanges();
            }

            return RedirectToAction("BooksReview", new { id = id });
        }

        public ActionResult DeleteBooksReview(int id)
        {
            var dbContext = new ApplicationDbContext();
            var info = dbContext.BooksReview.Find(id);
            int AssignId = info.AssignId;
            info.IsActive = false;
            info.ModifiedDate = DateTime.Now;
            dbContext.SaveChanges();

            Guid assignGuId = dbContext.BooksAssign.Find(AssignId).AssignGuId;

            return RedirectToAction("BooksReview",new {id= assignGuId }).WithSuccess("Review successfully deleted!");
        }

    }
}