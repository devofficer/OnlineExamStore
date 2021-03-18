using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Helpers;
using OnlineExam.Models;

namespace OnlineExam.Controllers
{
    [Authorize(Roles = "Admin, StaffAdmin")]
    public class VendorController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
            // return Json(vendors, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetVendors()
        {
            return Json(dbContext.Vendors.Where(x => x.IsActive).ToList()
                .Where(x => x.VendorCode != "JHSXX" && x.VendorCode != "BANK1"),//.SkipWhile(x => x.VendorCode == "JHSXX" && x.VendorCode == "BANK1"), 
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create(int? id)
        {
            //int ID = Id ?? 0;
            //ViewBag.CreateUpdate = ID == 0 ? "Create" : "Update";
            //QuestionCreation objQC = new QuestionCreation();
            //if (Id != null)
            //{
            //    objQC = GetQuestion_selected_Or_All(Id).FirstOrDefault(); // fetching selected question...
            //}
            //objQC.drpDifficultyLevel = Fill_DifficultyLevel(ID == 0 ? "-1" : objQC.DifficultyLevel);
            //objQC.drpDescription = QuestionPaperData(ID == 0 ? -1 : int.Parse(objQC.DifficultyLevel));

            //// ViewBag.Description = new SelectList(drp_Paper, "value", "Text");
            //return PartialView("_CreateQuestion", objQC);

            return PartialView("");
        }

        [HttpPost]
        public ActionResult Create(Vendor vendorViewModel)
        {
            if (ModelState.IsValid)
            {
                if (vendorViewModel != null)
                {
                    // UPDATE
                    try
                    {

                        if (dbContext.Vendors.Any(x => x.Id == vendorViewModel.Id && x.IsActive))
                        {
                            var vendorObj = dbContext.Vendors.FirstOrDefault(x => x.Id == vendorViewModel.Id && x.VendorCode == vendorViewModel.VendorCode && x.IsActive);
                            if (vendorObj != null)
                            {
                                vendorObj.Name = vendorViewModel.Name;
                                vendorObj.ContactNumber = vendorViewModel.ContactNumber;
                                vendorObj.ContactPerson = vendorViewModel.ContactPerson;
                                vendorObj.PrimaryEmail = vendorViewModel.PrimaryEmail;
                                vendorObj.SecondaryEmail = vendorViewModel.SecondaryEmail;
                                vendorObj.AddressLine1 = vendorViewModel.AddressLine1;
                                vendorObj.AddressLine2 = vendorViewModel.AddressLine2;
                                vendorObj.State = vendorViewModel.State;
                                vendorObj.Country = vendorViewModel.Country;
                                vendorObj.ZipCode = vendorViewModel.ZipCode;

                                vendorObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                vendorObj.ModifiedOn = DateTime.Now;
                                dbContext.Entry(vendorObj).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            // INSERT
                            vendorViewModel.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                            vendorViewModel.CreatedOn = DateTime.Now;
                            vendorViewModel.IsActive = true;
                            dbContext.Vendors.Add(vendorViewModel);
                            dbContext.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, errors = ex.Message });
                    }

                }
            }
            else
            {
                return Json(new { success = false, errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });
            }
            //return RedirectToAction("Index", "Vendor"); 
            // return Json(vendors, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, errors = "" });
        }

        public ActionResult Delete(int? id)
        {
            if (dbContext.Vendors.Any(x => x.Id == id))
            {
                var vendorObj = dbContext.Vendors.FirstOrDefault(x => x.Id == id && x.IsActive);
                if (vendorObj != null)
                {
                    // TBD: UPDATE IsActive = false
                    //dbContext.Vendors.Remove(vendorObj);
                    vendorObj.IsActive = false;
                    dbContext.Entry(vendorObj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            //return RedirectToAction("Index");
            return Json(dbContext.Vendors.Where(x => x.IsActive).ToList().SkipWhile(x => x.VendorCode == "JHSXX"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetVendorById(int? id)
        {
            var vendorObj = new Vendor { VendorCode = GlobalUtilities.RandomCode(5) };
            if (dbContext.Vendors.Any(x => x.Id == id && x.IsActive))
            {
                vendorObj = dbContext.Vendors.FirstOrDefault(x => x.Id == id && x.IsActive);
            }
            return PartialView("_Edit", vendorObj);
        }

    }
}