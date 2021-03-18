using System.Data.Entity;
using System.Threading;
using System.Transactions;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Models.ViewModels;
using EntityFramework.BulkInsert.Extensions;
using Remotion.Data.Linq.Clauses;
using OnlineExam.Repositories;

namespace OnlineExam.Controllers
{
    [Authorize(Roles = "Admin, StaffAdmin")]
    public class VoucherController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        //VoucherViewModel voucherObj = new VoucherViewModel();

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetVouchers()
        {
            //Status: 1 Not In Use, 2 Expired, 3 Used and Not Expired 
            
            var vouchers = (from vou in dbContext.Vouchers.Where(x => x.IsActive && x.VoucherCode != "VC0100")
                            join vendor in dbContext.Vendors on vou.VendorId equals vendor.Id
                            join up in dbContext.UserPlans on vou.Id equals up.VoucherId into a
                            from c in a.DefaultIfEmpty()
                            select new VoucherViewModel
                            {
                                Id = vou.Id,
                                VendorName = vendor.Name,
                                VoucherCode = vou.VoucherCode,
                                DateOfIssue = vou.DateOfIssue,
                                DateOfExpiry = vou.DateOfExpiry,
                                SelectedDenomination = vou.Denomination,
                                Status = c == null ? 1 : (c != null && (DbFunctions.DiffDays(c.ExpiryDate, DateTime.Now) > 0)) ? 2 : 3
                            }
                     ).ToList().OrderByDescending(x => x.Id);
            //var vouchers = dbContext.Vouchers
            //    .Join(dbContext.Vendors, voucher => voucher.VendorId, vendor => vendor.Id,
            //        (voucher, vendor) => new { Vouchers = voucher, Vendors = vendor })
            //    .Where(x => x.Vouchers.IsActive && x.Vouchers.VoucherCode != "VC0100")
            //    .ToList()
            //    .Select(x => new VoucherViewModel
            //    {
            //        Id = x.Vouchers.Id,
            //        VendorName = x.Vendors.Name,
            //        VoucherCode = x.Vouchers.VoucherCode,
            //        DateOfIssue = x.Vouchers.DateOfIssue,
            //        DateOfExpiry = x.Vouchers.DateOfExpiry,
            //        SelectedDenomination = x.Vouchers.Denomination
            //    }).ToList().OrderByDescending(x => x.Id);
            return Json(vouchers, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Create(VoucherViewModel voucherViewModel)
        {
            //voucherViewModel.SystemCode = GlobalUtilities.RandomCode(5) + voucherViewModel.SelectedDenomination;
            //voucherViewModel.VoucherCode = GlobalUtilities.RandomCode(5);
            if (ModelState.IsValid)
            {
                if (voucherViewModel != null)
                {
                    if (dbContext.Vouchers.Any(x => x.Id == voucherViewModel.Id && x.VoucherCode == voucherViewModel.VoucherCode && x.IsActive))
                    {
                        var voucherObj = dbContext.Vouchers.FirstOrDefault(x => x.Id == voucherViewModel.Id && x.IsActive);
                        if (voucherObj != null)
                        {
                            voucherObj.DateOfExpiry = voucherViewModel.DateOfExpiry;
                            voucherObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                            voucherObj.ModifiedOn = DateTime.Now;
                            dbContext.Entry(voucherObj).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        // INSERT

                        // VENDOR CODE + DENOMINATION + AUTO GENERATED 5 DIGITS CODE
                        //voucher.SystemCode = voucherViewModel.SystemCode;
                        //voucher.VoucherCode = voucherViewModel.VoucherCode;
                        //var vouchers = new List<Voucher>();
                        //for (int i = 0; i < voucherViewModel.NoOfVoucher; i++)
                        //{
                        //    var voucher = new Voucher();
                        //    voucher.SystemCode = GlobalUtilities.RandomCode(5) + voucherViewModel.SelectedDenomination;
                        //    voucher.VoucherCode = GlobalUtilities.RandomCode(5);
                        //    voucher.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                        //    voucher.CreatedOn = DateTime.Now;
                        //    voucher.IsActive = true;
                        //    voucher.VendorId = voucherViewModel.SelectedVendorId;
                        //    voucher.MembershipPlanId = voucherViewModel.SelectedMembershipPlanId;
                        //    voucher.Denomination = voucherViewModel.SelectedDenomination;
                        //    voucher.DateOfIssue = DateTime.Now;
                        //    voucher.DateOfExpiry = DateTime.Now.AddDays(30);
                        //    vouchers.Add(voucher);
                        //}

                        try
                        {


                            using (var transactionScope = new TransactionScope())
                            {
                                // some stuff in dbcontext

                                //dbContext.BulkInsert(vouchers);
                                //var voucher = new Voucher();
                                //voucher.SystemCode = GlobalUtilities.RandomCode(5) + voucherViewModel.SelectedDenomination;
                                //voucher.VoucherCode = GlobalUtilities.RandomCode(5);
                                //voucher.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                                //voucher.CreatedOn = DateTime.Now;
                                //voucher.IsActive = true;
                                //voucher.VendorId = voucherViewModel.SelectedVendorId;
                                //voucher.MembershipPlanId = voucherViewModel.SelectedMembershipPlanId;
                                //voucher.Denomination = voucherViewModel.SelectedDenomination;
                                //voucher.DateOfIssue = DateTime.Now;
                                //voucher.DateOfExpiry = DateTime.Now.AddDays(30);

                                //dbContext.Vouchers.Add(voucher);
                                var voucherCodeDigitCounter = CommonRepository.GetVoucherCodeDigitCounter();
                                for (int i = 0; i < voucherViewModel.NoOfVoucher; i++)
                                {
                                    var voucher = new Voucher();
                                    voucher.SystemCode = GlobalUtilities.RandomCode(voucherCodeDigitCounter) + voucherViewModel.SelectedDenomination;
                                    voucher.VoucherCode = GlobalUtilities.RandomCode(voucherCodeDigitCounter);
                                    voucher.CreatedBy = CustomClaimsPrincipal.Current.UserId;
                                    voucher.CreatedOn = DateTime.Now;
                                    voucher.IsActive = true;
                                    voucher.VendorId = voucherViewModel.SelectedVendorId;
                                    voucher.MembershipPlanId = voucherViewModel.SelectedMembershipPlanId;
                                    voucher.Denomination = voucherViewModel.SelectedDenomination;
                                    voucher.DateOfIssue = DateTime.Now;
                                    
                                    // VOUCHER EXPIRY DATE SHOULD BE SET WHEN ITS BEING USED  
                                    //voucher.DateOfExpiry = voucherViewModel.SelectedDenomination == 1200 ? DateTime.Now.AddDays(366) : DateTime.Now.AddDays(30);
                                    Thread.Sleep(500);
                                    dbContext.Vouchers.Add(voucher);
                                }
                                dbContext.SaveChanges();
                                transactionScope.Complete();
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { success = false, errors = ex.Message });
                        }
                    }
                }
            }
            else
            {
                return Json(new { success = false, errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });
            }
            return Json(new { success = true, errors = "" });
        }
        public ActionResult Delete(int? id)
        {
            try
            {
                if (dbContext.Vouchers.Any(x => x.Id == id && x.IsActive))
                {
                    var voucherObj = dbContext.Vouchers.FirstOrDefault(x => x.Id == id && x.IsActive);
                    if (voucherObj != null)
                    {
                        voucherObj.IsActive = false;
                        dbContext.Entry(voucherObj).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
            return Json(new { success = true, errors = "" });
        }
    }
}