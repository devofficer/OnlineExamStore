using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using OnlineExam.Helpers;
using System.Net.Mail;
using System.Net;
using System.Data.Entity.Validation;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Data.SqlTypes;

namespace OnlineExam.Controllers
{
    [TraceError()]
    [Authorize]
    public class AccountController : Controller
    {
        IUnitOfWork unitOfWork = new UnitOfWork();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            var signInStatus = SignInStatus.Success;
            ViewBag.UserPlanType = "Trial";
            // CHECK, IF LOGGED-IN USER IS ADMIN OR STAFF
            if (!CustomClaimsPrincipal.Current.CurrentUserEmail.Contains(AppConstants.DomainName))
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var userPlanObj =
                        dbContext.UserPlans.Include("MembershipPlan")
                            .FirstOrDefault(x => x.IsActive && x.UserId == CustomClaimsPrincipal.Current.UserId);
                    if (userPlanObj != null && userPlanObj.MembershipPlan != null)
                    {
                        #region MyRegion
                        ViewBag.UserPlanType = userPlanObj.MembershipPlan.Name;

                        if (userPlanObj.ExpiryDate.HasValue)
                        {
                            int compareValue =
                                Convert.ToDateTime(userPlanObj.ExpiryDate).Date.CompareTo(DateTime.Today);
                            if (compareValue < 0)
                                signInStatus = SignInStatus.MembershipExpired;
                            else if (compareValue == 0 || compareValue > 0)
                                signInStatus = SignInStatus.Success;
                        }
                        else
                        {
                            // First login
                            userPlanObj.ExpiryDate = DateTime.Now.AddDays(userPlanObj.MembershipPlan.ValidityInDays);
                            userPlanObj.ModifiedBy = CustomClaimsPrincipal.Current.CurrentUserEmail;
                            userPlanObj.ModifiedOn = DateTime.Now;

                            dbContext.Entry(userPlanObj).State = EntityState.Modified;
                            if (dbContext.SaveChanges() > 0)
                            {
                                signInStatus = SignInStatus.Success;
                            }
                        }

                        #endregion
                    }
                    else
                        signInStatus = SignInStatus.MembershipExpired;
                }
            }
            if (signInStatus == SignInStatus.Success)
            {
                ViewBag.UserId = CustomClaimsPrincipal.Current.UserId;

                

                //int i = int.Parse("ss");
                return View();
            }
            else
            {
                return RedirectToAction("MembershipExpired");
            }
        }
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (CustomClaimsPrincipal.Current != null && !string.IsNullOrWhiteSpace(CustomClaimsPrincipal.Current.UserId)) return RedirectToAction("Dashboard", "Account");
            Session["dummy"] = "dummy"; // Create ASP.NET_SessionId cookie
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LicenseAgreementMsg = AppConstants.LicenseAgreementMsg;
            return View();
        }

        private SignInHelper _helper;

        private SignInHelper SignInHelper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _helper;
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInHelper.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });

                case SignInStatus.EmailNotVerified:
                    ViewBag.UserId = UserManager.FindByEmail(model.Email).Id;
                    ViewBag.EmailVerificationCode = await UserManager.GenerateEmailConfirmationTokenAsync(UserManager.FindByEmail(model.Email).Id);
                    return View("EmailNotVerified");
                case SignInStatus.MembershipExpired:
                    return RedirectToAction("MembershipExpired");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        public ActionResult MembershipExpired()
        {
            var userPlanViewModel = new UserPlanViewModel();
            using (var dbContext = new ApplicationDbContext())
            {
                var userBankPaymentsObj = dbContext.UserBankPayments.FirstOrDefault(x => x.UserId == CustomClaimsPrincipal.Current.UserId
                    && x.PaymentStatus == PaymentStatus.Confirmed);
                if (userBankPaymentsObj != null)
                {
                    userPlanViewModel.PaymentStatus = userBankPaymentsObj.PaymentStatus;
                }
                else
                {
                    var voucherCode = GlobalUtilities.RandomCode(5);
                    userPlanViewModel.Despositor = userPlanViewModel.Narration = userPlanViewModel.TxnId = voucherCode;
                }
            }
            return View(userPlanViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MembershipExpired(UserPlanViewModel userPlanViewModel, string Command)
        {
            var voucherObj = new Voucher();
            var membershipPlanObj = new MembershipPlan();
            if (Command == "VoucherPayment")
            {
                if (userPlanViewModel.VoucherCode == CommonRepository.GetDemoVoucher().VoucherCode)
                {
                    ModelState.AddModelError("", "The provided Voucher Code is invalid.");
                    return View(userPlanViewModel);
                }
                voucherObj = CommonRepository.GetVoucherByCode(userPlanViewModel.VoucherCode);
                membershipPlanObj = CommonRepository.GetMembershipPlan(AppConstants.RegisterType.Paid);

                if (voucherObj == null || membershipPlanObj == null || voucherObj.Denomination < 1200)
                {
                    ModelState.AddModelError("", "The provided Voucher Code is either expired or invalid.");
                    return View(userPlanViewModel);
                }
            }
            else if (Command == "BankPayment")
            {
                ModelState.Remove("VoucherCode");
            }

            if (ModelState.IsValid)
            {
                if (Command == "VoucherPayment")
                {
                    #region VOUCHER PAYMENT
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var userPlanObj = dbContext.UserPlans.FirstOrDefault(x => x.IsActive && x.UserId == CustomClaimsPrincipal.Current.UserId);
                        if (userPlanObj != null)
                        {
                            userPlanObj.IsActive = false;
                            userPlanObj.ExpiryDate = DateTime.Now;
                            userPlanObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                            userPlanObj.ModifiedOn = DateTime.Now;
                            dbContext.Entry(userPlanObj).State = EntityState.Modified;
                        }
                        dbContext.UserPlans.Add(new UserPlan
                        {
                            UserId = CustomClaimsPrincipal.Current.UserId,
                            VoucherId = voucherObj.Id,
                            MembershipPlanId = membershipPlanObj.Id,
                            IsActive = true,
                            ExpiryDate = DateTime.Now.AddDays(membershipPlanObj.ValidityInDays),
                            CreatedBy = CustomClaimsPrincipal.Current.UserId,
                            CreatedOn = DateTime.Now,
                        });
                        //ALSO UPDATE VOUCHER EXPIRY DATE
                        voucherObj.DateOfExpiry = voucherObj.Denomination == 1200 ? DateTime.Now.AddDays(366) : DateTime.Now.AddDays(30);
                        dbContext.Entry(voucherObj).State = EntityState.Modified;

                        using (var txn = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                                var identity = new ClaimsIdentity(User.Identity);
                                if (identity.Claims.Any(c => c.Type == "MembershipPlan"))
                                {
                                    identity.RemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == "MembershipPlan"));
                                    identity.AddClaim(new Claim("MembershipPlan", membershipPlanObj.Name));
                                }
                                if (identity.Claims.Any(c => c.Type == "MembershipPlanCode"))
                                {
                                    identity.RemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == "MembershipPlanCode"));
                                    identity.AddClaim(new Claim("MembershipPlanCode", membershipPlanObj.MembershipPlanCode));
                                }
                                authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant
                                          (new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });
                                dbContext.SaveChanges();
                                txn.Commit();
                            }
                            catch
                            {
                                txn.Rollback();
                                return View(userPlanViewModel).WithError(AppConstants.ErrorMessageText);
                            }
                        }
                    }
                    return RedirectToAction("Dashboard", "Account").WithSuccess("Your Membership Plan has been Upgraded successfully.");
                    #endregion
                }
                if (Command == "BankPayment")
                {
                    #region BANK PAYMENT
                    using (var dbContext = new ApplicationDbContext())
                    {
                        // INSERT RECORD INTO USERBANKACCOUNT TABLE
                        dbContext.UserBankPayments.Add(new UserBankPayment
                        {
                            UserId = CustomClaimsPrincipal.Current.UserId,
                            TxnId = userPlanViewModel.TxnId,// VOUCHER CODE
                            Amount = userPlanViewModel.Amount,
                            Account = userPlanViewModel.Account,
                            Beneficiary = userPlanViewModel.Beneficiary,
                            PaymentStatus = Utils.PaymentStatus.Confirmed,
                            Bank = userPlanViewModel.Bank,
                            IsActive = true,
                            CreatedOn = DateTime.UtcNow
                        });

                        // INSERT RECORD INTO VOUCHER TABLE
                        var voucherCodeDigitCounter = CommonRepository.GetVoucherCodeDigitCounter();
                        if (voucherCodeDigitCounter == 0)
                        {
                            voucherCodeDigitCounter = 8;
                        }
                        dbContext.Vouchers.Add(new Voucher
                        {
                            SystemCode = GlobalUtilities.RandomCode(voucherCodeDigitCounter) + 1500,
                            VoucherCode = userPlanViewModel.TxnId,// VOUCHER CODE
                            CreatedBy = CustomClaimsPrincipal.Current.UserId,
                            CreatedOn = DateTime.UtcNow,
                            IsActive = true,
                            VendorId = 3,
                            MembershipPlanId = 0,
                            Denomination = 1500,
                            DateOfIssue = DateTime.UtcNow
                        });
                        using (var txn = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                dbContext.SaveChanges();
                                txn.Commit();
                            }
                            catch
                            {
                                txn.Rollback();
                                return View(userPlanViewModel).WithError(AppConstants.ErrorMessageText);
                            }
                        }
                        return RedirectToAction("Dashboard", "Account");
                    }
                    #endregion
                }
            }
            return View(userPlanViewModel);
        }

        [Authorize(Roles = "Admin, StaffAdmin")]
        public ActionResult ManageBankPayment()
        {
            return View(new UserBankPayment());
        }
        [HttpGet]
        public ActionResult GetUserBankPayments()
        {
            var userBankPayments = GetBankPayments();
            return Json(userBankPayments, JsonRequestBehavior.AllowGet);
        }

        private static IOrderedEnumerable<BankPaymentViewModel> GetBankPayments()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userBankPayments = (from payment in dbContext.UserBankPayments
                                        join profile in dbContext.UserProfiles on payment.UserId equals profile.ApplicationUser.Id
                                        select new BankPaymentViewModel
                                        {
                                            Id = payment.Id,
                                            Email = profile.Email,
                                            UserName = profile.FirstName + " " + profile.LastName,
                                            TxnId = payment.TxnId,
                                            PaymentStatus = payment.PaymentStatus,
                                            Beneficiary = payment.Beneficiary,
                                            Bank = payment.Bank,
                                            Account = payment.Account,
                                            CreatedOn = payment.CreatedOn
                                        }
                     ).ToList().OrderByDescending(x => x.Id);
                return userBankPayments;
            }
        }

        [Authorize(Roles = "Admin, StaffAdmin")]
        [HttpPost]
        public ActionResult ApprovePayment(int id, int approvedStatus)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var paymentStatusObj = PaymentStatus.None;
                switch (approvedStatus)
                {
                    case 1: paymentStatusObj = PaymentStatus.Confirmed; break;
                    case 2: paymentStatusObj = PaymentStatus.Pending; break;
                    case 3: paymentStatusObj = PaymentStatus.Received; break;
                }

                // UPDATE PAYMENT STATUS 
                var userBankPaymentsObj = dbContext.UserBankPayments.FirstOrDefault(x => x.Id == id && x.IsActive);
                if (userBankPaymentsObj != null)
                {
                    userBankPaymentsObj.PaymentStatus = paymentStatusObj;
                    userBankPaymentsObj.PaymentOn = DateTime.UtcNow;
                    dbContext.Entry(userBankPaymentsObj).State = EntityState.Modified;
                }

                // UPDATE MEMBERSHIP PLAN 
                var voucherObj = CommonRepository.GetVoucherByCode(userBankPaymentsObj.TxnId);
                var membershipPlanObj = CommonRepository.GetMembershipPlan(AppConstants.RegisterType.Paid);
                var userPlanObj = dbContext.UserPlans.FirstOrDefault(x => x.IsActive && x.UserId == userBankPaymentsObj.UserId);
                if (userPlanObj != null)
                {
                    userPlanObj.IsActive = false;
                    userPlanObj.ExpiryDate = DateTime.UtcNow;
                    userPlanObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                    userPlanObj.ModifiedOn = DateTime.UtcNow;
                    dbContext.Entry(userPlanObj).State = EntityState.Modified;
                }
                dbContext.UserPlans.Add(new UserPlan
                {
                    UserId = userPlanObj.UserId, // USERID FOR USER FOR WHICH MEMBERSHIP PLAN IS UPGRADED
                    VoucherId = voucherObj.Id,
                    MembershipPlanId = membershipPlanObj.Id,
                    IsActive = true,
                    ExpiryDate = DateTime.Now.AddDays(membershipPlanObj.ValidityInDays),
                    CreatedBy = CustomClaimsPrincipal.Current.UserId,
                    CreatedOn = DateTime.UtcNow,
                });
                //ALSO UPDATE VOUCHER EXPIRY DATE
                voucherObj.MembershipPlanId = membershipPlanObj.Id;
                voucherObj.DateOfExpiry = voucherObj.Denomination == 1500 ? DateTime.Now.AddDays(366) : DateTime.Now.AddDays(30);
                dbContext.Entry(voucherObj).State = EntityState.Modified;

                using (var txn = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbContext.SaveChanges();
                        txn.Commit();
                    }
                    catch (Exception ex)
                    {
                        txn.Rollback();
                        return Json(new { success = false, errors = ex.Message });
                    }
                }
                var userBankPayments = GetBankPayments();
                return Json(new { success = true, errors = "", data = userBankPayments }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ReferralPragram()
        {
            return View(new ReferralPragramViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> ReferralPragram(ReferralPragramViewModel referralPragramViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var isExistUser = await UserManager.FindByEmailAsync(referralPragramViewModel.Email);
                    if (isExistUser != null)
                    {
                        ModelState.AddModelError("", AppConstants.EmailIsExistMessage);
                        return View(referralPragramViewModel).WithError(AppConstants.EmailIsExistMessage);
                    }
                    #region SEND EMAIL
                    try
                    {
                        var code = await UserManager.GenerateUserTokenAsync("Refer a Friend", CustomClaimsPrincipal.Current.UserId);
                        var callbackUrl = Url.Action("PreRegister", "Account", new { code = code }, protocol: Request.Url.Scheme);
                        var referralLevelTwo = "";
                        var email = UserManager.GetEmail(CustomClaimsPrincipal.Current.UserId);
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            var referralUserObj = dbContext.ReferralPragrams.FirstOrDefault(x => x.Email == email && x.IsActive);
                            if (referralUserObj != null)
                            {
                                referralLevelTwo = referralUserObj.ReferralLevelOne;
                            }
                        }
                        dbContext.ReferralPragrams.Add(new ReferralPragram
                        {
                            Email = referralPragramViewModel.Email,
                            ReferralUrl = callbackUrl,
                            ReferralLevelOne = CustomClaimsPrincipal.Current.CurrentUserEmail,
                            ReferralLevelTwo = referralLevelTwo,
                            ReferralCode = code,
                            CreatedBy = CustomClaimsPrincipal.Current.UserId,
                            CreatedOn = DateTime.UtcNow,
                            ModifiedBy = string.Empty
                        });
                        dbContext.SaveChanges();
                        var body = EmailTemplates.GetEmailUser_ReferralEmail(CustomClaimsPrincipal.Current.UserFullName, callbackUrl);
                        //await UserManager.SendEmailAsync(CustomClaimsPrincipal.Current.UserId, "ACADAStore: Referred For ACADASTORE", body);
                        EmailService.SendMail(referralPragramViewModel.Email, body, "ACADAStore: Referred For ACADASTORE");
                        ViewBag.IsEmailSent = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLogFile("ReferralPragram", ex.Message, ex.StackTrace);
                        ViewBag.IsEmailSent = false;
                    }
                    return View("DisplayEmail");
                    #endregion
                }
            }
            return View(referralPragramViewModel);
        }

        [HttpGet]
        public ActionResult ReferralList()
        {
            ViewBag.ReferralCounter = CommonRepository.GetReferralCounter();
            return View(new ReferralViewModel());
        }
        [HttpGet]
        public ActionResult GetReferrals(bool is1stLevelReferral)
        {
            var referrals = new List<ReferralViewModel>();
            var paidOrders = new List<ReferralOrderViewModel>();
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                  using(var multipleResult = db.QueryMultiple("GetReferralResultSet",
                      new {loggedInUserEmail = CustomClaimsPrincipal.Current.CurrentUserEmail,Is1stLevelReferral = is1stLevelReferral},
                      commandType: CommandType.StoredProcedure))
                  {
                      referrals = multipleResult.Read<ReferralViewModel>().ToList();
                      paidOrders = multipleResult.Read<ReferralOrderViewModel>().ToList();
                  }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }

            return Json(new { success = true, errors = "", data = referrals, paidOrders = paidOrders }, JsonRequestBehavior.AllowGet);


            //var referrals = GetReferralList(is1stLevelReferral);
            //return Json(referrals, JsonRequestBehavior.AllowGet);
        }

        private static List<ReferralViewModel> GetReferralList(bool is1stLevelReferral)
        {
            var referrals = new List<ReferralViewModel>();
            var email = CustomClaimsPrincipal.Current.CurrentUserEmail;
            using (var dbContext = new ApplicationDbContext())
            {
                var emailAddress = new SqlParameter("@LoggedInUserEmail", string.IsNullOrEmpty(email) ? DBNull.Value.ToString() : email);
                var is1sTLevelReferral = new SqlParameter("@Is1stLevelReferral", is1stLevelReferral);
                referrals = dbContext.Database.SqlQuery<ReferralViewModel>("GetReferralResultSet @LoggedInUserEmail, @Is1stLevelReferral", emailAddress, is1sTLevelReferral).ToList();
            }
            return referrals;
        }

        [Authorize(Roles = "Admin, StaffAdmin")]
        public ActionResult ManageBonusOrder()
        {
            return View();
        }
        [Authorize(Roles = "Admin, StaffAdmin")]
        [HttpPost]
        public ActionResult BonusPayment(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    var result = db.Execute("UpdateBonusOrder", new { referralOrderId = id, userId = CustomClaimsPrincipal.Current.UserId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }

            var orders = GetOrders();
            return Json(new { success = true, errors = "", data = orders }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetBonusOrders()
        {
            var orders = GetOrders();
            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        private static IOrderedEnumerable<ReferralOrderViewModel> GetOrders()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var orders = (from rd in dbContext.ReferralOrders
                              join profile in dbContext.UserProfiles on rd.CreatedBy equals profile.Email
                              where rd.PaymentStatus == Enums.ReferralPaymentStatus.Pending.ToString()
                              select new ReferralOrderViewModel
                              {
                                  Id = rd.Id,
                                  ReferralPragramIds = rd.ReferralPragramIds,
                                  Email = profile.Email,
                                  Name = profile.FirstName + " " + profile.LastName,
                                  PaymentStatus = rd.PaymentStatus,
                                  ReferralType = rd.ReferralType,
                                  TotalBonus = rd.TotalBonus,
                                  CreatedOn = rd.CreatedOn
                              }
             ).ToList().OrderByDescending(x => x.CreatedOn);
                return orders;
            }
        }


        [HttpGet]
        public ActionResult GetRedeemOrder(bool is1stLevelReferral)
        {
            var isOrderPlaced = false;
            //var isBankUsersFound = false;
            try
            {
                var orderQueryString = "";
                var bankPaidUserQueryString = "";
                
                if (is1stLevelReferral)
                {
                    //bankPaidUserQueryString = "SELECT COUNT(*) AS BankUserCount FROM AspNetUsers AU" +
                    //            " JOIN UserPlan UP ON AU.Id = UP.UserId  AND UP.IsActive = 1" +
                    //            " JOIN Voucher V ON UP.VoucherId = V.Id  AND V.IsActive = 1" +
                    //            " JOIN ReferralPragram RP ON AU.Email = RP.Email" +
                    //            " WHERE RP.ReferralLevelOne='" + CustomClaimsPrincipal.Current.CurrentUserEmail + "'" +
                    //            " AND UP.MembershipPlanId = 2 AND V.VendorId = 3";

                    orderQueryString = "SELECT COUNT(*) AS BankUserCount FROM ReferralOrder WHERE CreatedBy='" + CustomClaimsPrincipal.Current.CurrentUserEmail + "'" +
                                    " AND PaymentStatus='" + OnlineExam.Infrastructure.Enums.ReferralPaymentStatus.Pending.ToString() + "'" +
                                    " AND ReferralType = '" + OnlineExam.Infrastructure.Enums.ReferralType.FirstLevel.ToString() + "'";
                }
                else
                {
                    //bankPaidUserQueryString = "SELECT COUNT(*) AS BankUserCount FROM AspNetUsers AU" +
                    //            " JOIN UserPlan UP ON AU.Id = UP.UserId  AND UP.IsActive = 1" +
                    //            " JOIN Voucher V ON UP.VoucherId = V.Id  AND V.IsActive = 1" +
                    //            " JOIN ReferralPragram RP ON AU.Email = RP.Email" +
                    //            " WHERE RP.ReferralLevelTwo='" + CustomClaimsPrincipal.Current.CurrentUserEmail + "'" +
                    //            " AND UP.MembershipPlanId = 2 AND V.VendorId = 3";

                    orderQueryString = "SELECT COUNT(*) AS BankUserCount FROM ReferralOrder WHERE CreatedBy='" + CustomClaimsPrincipal.Current.CurrentUserEmail + "'" +
                                    " AND PaymentStatus='" + OnlineExam.Infrastructure.Enums.ReferralPaymentStatus.Pending.ToString() + "'" +
                                    " AND ReferralType = '" + OnlineExam.Infrastructure.Enums.ReferralType.SecondLevel.ToString() + "'";
                }


                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    var orderFound = db.ExecuteScalar<int>(orderQueryString);
                    //var bankUsersCount = db.ExecuteScalar<int>(bankPaidUserQueryString);

                    isOrderPlaced = orderFound > 0;
                    //isBankUsersFound = bankUsersCount > 0;
                }

                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
            return Json(new { success = true, errors = "", isOrderPlaced = isOrderPlaced }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PlaceRedeemOrder(List<PostReferralViewModel> referralList, bool is1stLevelReferral)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                try
                {
                    var ids = string.Join("|", referralList.Select(x => x.Id));
                    var recordCount = Convert.ToInt32(referralList.Select(x => x.IsPaidByBank).Count());
                    var bonus = Convert.ToDecimal(referralList.Sum(x => x.Bonus));
                    var level = is1stLevelReferral ? OnlineExam.Infrastructure.Enums.ReferralType.FirstLevel.ToString() : OnlineExam.Infrastructure.Enums.ReferralType.SecondLevel.ToString();

                    //var referralPragramIds = new SqlParameter("@ReferralPragramIds", string.IsNullOrEmpty(ids) ? DBNull.Value.ToString() : ids);
                    //var referralType = new SqlParameter("@ReferralType", string.IsNullOrEmpty(level) ? DBNull.Value.ToString() : level);
                    //var totalReferralCount = new SqlParameter("@TotalReferralCount", recordCount);
                    //var totalBonus = new SqlParameter("@TotalBonus", bonus);
                    //var userId = new SqlParameter("@CreatedBy", CustomClaimsPrincipal.Current.UserId);

                    var p1 = new SqlParameter { ParameterName = "@ReferralPragramIds", SqlDbType = SqlDbType.VarChar, Value = ids };
                    var p2 = new SqlParameter { ParameterName = "@ReferralType", SqlDbType = SqlDbType.VarChar, Value = level };
                    var p3 = new SqlParameter { ParameterName = "@TotalReferralCount", SqlDbType = SqlDbType.Int, Value = recordCount };
                    var p4 = new SqlParameter { ParameterName = "@TotalBonus", SqlDbType = SqlDbType.Decimal, Value = bonus };
                    var p5 = new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.VarChar, Value = CustomClaimsPrincipal.Current.CurrentUserEmail };

                    var i = dbContext.Database.ExecuteSqlCommand("InsertReferralOrder @ReferralPragramIds, @ReferralType, @TotalReferralCount, @TotalBonus, @UserId",
                        p1, p2, p3, p4, p5);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = ex.Message });
                }
            }
            //var referrals = GetReferralList(is1stLevelReferral);
            var redeemOrderObj = GetRedeemOrder(is1stLevelReferral);
            var isOrderPopopShow = false;

            if (redeemOrderObj != null)
            {
                isOrderPopopShow = true;
            }

            return Json(new { success = true, errors = "", isShowPopup = isOrderPopopShow }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInHelper.HasBeenVerified())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInHelper.GetVerifiedUserIdAsync());
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInHelper.TwoFactorSignIn(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        /// <summary>
        /// for seletcting register type
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PreRegister(string code = "")
        {
            if (CustomClaimsPrincipal.Current != null && !string.IsNullOrWhiteSpace(CustomClaimsPrincipal.Current.UserId)) return RedirectToAction("Dashboard", "Account");

            return View();
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string register, string code = "")
        {
            var registerViewModel = new RegisterViewModel
            {
                UserTypes = register== "Paid"?CommonRepository.GetLookups(Enums.LookupType.UserType.ToString()).Where(x=>x.Value != "Teacher"): CommonRepository.GetLookups(Enums.LookupType.UserType.ToString()),
                ClasseTypes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString()),
               // SubjectCategory = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString()).DistinctBy(x=>x.Value)
            };
            using (var dbContext = new ApplicationDbContext())
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (dbContext.ReferralPragrams.Any(x => x.ReferralCode == code && !x.IsActive))
                    {
                        var referralPragramsObj = dbContext.ReferralPragrams.FirstOrDefault(x => x.ReferralCode == code && !x.IsActive);
                        if (referralPragramsObj != null)
                        {
                            registerViewModel.Email = referralPragramsObj.Email;
                            registerViewModel.ReferralCode = referralPragramsObj.ReferralCode;
                        }
                    }
                }
            }
            switch (register)
            {
                case AppConstants.RegisterType.Demo:
                    registerViewModel.RegisterType = @ViewBag.RegisterType = AppConstants.RegisterType.Demo;
                    registerViewModel.VoucherCode = CommonRepository.GetDemoVoucher().VoucherCode;
                    break;
                case AppConstants.RegisterType.Paid:
                    registerViewModel.RegisterType = @ViewBag.RegisterType = AppConstants.RegisterType.Paid;
                    break;
                default:
                    return RedirectToAction("PreRegister");
            }

            return View(registerViewModel);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string register)
        {
            if (model.UserType == "Student")
            {
                if (model.SelectedClasses != null && model.SelectedClasses.Count() > 1)
                {
                    ModelState.AddModelError("", "Only one Class is allowed.");
                }
            }

            if (model.UserType == "Student")
            {
                if (model.SelectedClasses == null || !model.SelectedClasses.Any())
                {
                    ModelState.AddModelError("", "Please select a Class.");
                }
            }

            if (model.UserType == "Teacher")
            {
                if (model.SubjectCategories == null || !model.SubjectCategories.Any())
                {
                    ModelState.AddModelError("", "Please select a Category.");
                }
            }

            model.RegisterType = register.ToLower() == "demo" ? AppConstants.RegisterType.Demo : AppConstants.RegisterType.Paid;

            //if (model.RegisterType == AppConstants.RegisterType.Demo)
            //{
            //    model.VoucherCode = CommonRepository.GetDemoVoucher().VoucherCode;
            //}
            if (string.IsNullOrWhiteSpace(model.VoucherCode) && model.RegisterType == AppConstants.RegisterType.Paid)
            {
                ModelState.AddModelError("", "Please provide the Voucher Code.");
            }

            Voucher voucherObj = null;

            var voucherDemoObj = CommonRepository.GetDemoVoucher();

            if (model.VoucherCode != voucherDemoObj.VoucherCode)
                voucherObj = CommonRepository.GetVoucherByCode(model.VoucherCode);

            if (voucherDemoObj == null)
            {
                ModelState.AddModelError("", "The Demo Voucher Code is invalid or doesn't eixts or being used.");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(model.VoucherCode) &&
                    model.RegisterType == AppConstants.RegisterType.Paid)
                {
                    if (voucherObj == null)
                    {
                        ModelState.AddModelError("", "The provided Voucher Code is either invalid or expired.");
                    }
                    else if (model.VoucherCode == voucherDemoObj.VoucherCode)
                    {
                        ModelState.AddModelError("", "The provided Voucher Code is invalid or doesn't eixts.");
                    }
                }
                else
                {
                    model.VoucherCode = voucherDemoObj.VoucherCode;
                    ModelState["VoucherCode"].Errors.Clear();
                }
            }
            var isExistUser = await UserManager.FindByEmailAsync(model.Email);
            if (isExistUser != null)
            {
                ModelState.AddModelError("", AppConstants.EmailIsExistMessage);
                //return View(model);
            }
            if (ModelState.IsValid)
            {
                var userPlan = new UserPlan { CreatedOn = DateTime.Now, CreatedBy = CustomClaimsPrincipal.Current.CurrentUserEmail, IsActive = true };
                switch (model.RegisterType)
                {
                    case AppConstants.RegisterType.Demo:
                        userPlan.VoucherId = voucherDemoObj.Id;
                        userPlan.MembershipPlanId = CommonRepository.GetMembershipPlan(AppConstants.RegisterType.Demo).Id;
                        break;
                    case AppConstants.RegisterType.Paid:
                        userPlan.VoucherId = voucherObj.Id;
                        userPlan.MembershipPlanId = CommonRepository.GetMembershipPlan(AppConstants.RegisterType.Paid).Id;
                        //ALSO UPDATE VOUCHER EXPIRY DATE
                        voucherObj.DateOfExpiry = voucherObj.Denomination == 1200 ? DateTime.Now.AddDays(366) : DateTime.Now.AddDays(30);
                        break;
                }
                // bool isEmailAusVisa= model.Email.Split('@')[1] == AppConstants.DomainName;

                var user = new ApplicationUser
                {
                    IsAgreementAccpeted = true,
                    UserType = model.UserType,
                    UserName = model.Email,
                    Email = model.Email,
                    //ReferedBy = AppConstants.MyRMA,
                    Status = AppConstants.UserStatus.Created,
                    UserProfile = model.UserType=="Teacher"? new UserProfile { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, DOB = DateTime.UtcNow, ClassTypes = string.Join("|", model.SelectedClasses), SubjectCategory = string.Join("|", model.SubjectCategories) }: new UserProfile { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, DOB = DateTime.UtcNow, ClassTypes = string.Join("|", model.SelectedClasses) },
                    UserPlans = new List<UserPlan> { userPlan }
                };
                try
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        result = UserManager.AddToRole(user.Id, model.UserType);
                        if (result.Succeeded)
                        {
                            using (var dbContext = new ApplicationDbContext())
                            {
                                using (var txn = dbContext.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        //ALSO UPDATE VOUCHER EXPIRY DATE, IF VOUCHER IS NON DEMO TYPE
                                        #region UPDATE VOUCHER
                                        if (model.VoucherCode != voucherDemoObj.VoucherCode &&
                                                               model.RegisterType == AppConstants.RegisterType.Paid)
                                        {
                                            dbContext.Entry(voucherObj).State = EntityState.Modified;
                                        }
                                        #endregion
                                        #region REFERRAL PRAGRAMS ACTIVATION
                                        if (!string.IsNullOrWhiteSpace(model.ReferralCode))
                                        {
                                            if (dbContext.ReferralPragrams.Any(x => x.ReferralCode == model.ReferralCode && !x.IsActive))
                                            {
                                                var referralPragramsObj = dbContext.ReferralPragrams.FirstOrDefault(x => x.ReferralCode == model.ReferralCode && !x.IsActive);
                                                if (referralPragramsObj != null)
                                                {
                                                    referralPragramsObj.IsActive = true;
                                                    referralPragramsObj.ModifiedOn = DateTime.UtcNow;
                                                    dbContext.Entry(referralPragramsObj).State = EntityState.Modified;
                                                }
                                            }
                                        }
                                        #endregion

                                        #region SAVE REFFERAL
                                        if (!string.IsNullOrWhiteSpace(model.ReferrerEmail))
                                        {
                                            var referralLevelTwo = "";
                                            var referralPragramsObj = dbContext.ReferralPragrams.FirstOrDefault(x => x.Email == model.ReferrerEmail);
                                            if (referralPragramsObj != null)
                                            {
                                                referralLevelTwo = referralPragramsObj.ReferralLevelOne;
                                            }

                                            dbContext.ReferralPragrams.Add(new ReferralPragram
                                            {
                                                Email = model.Email,
                                                ReferralLevelOne = model.ReferrerEmail,
                                                ReferralLevelTwo = referralLevelTwo,
                                                IsActive = true,
                                                CreatedOn = DateTime.UtcNow,
                                                CreatedBy = user.Id
                                            });
                                        }
                                        #endregion
                                        dbContext.SaveChanges();
                                        txn.Commit();

                                        #region SEND EMAIL
                                        try
                                        {
                                            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                                            var userName = model.FirstName + " " + model.LastName;
                                            var body = EmailTemplates.GetEmailUser_VerifyEmail(userName, callbackUrl);
                                            await UserManager.SendEmailAsync(user.Id, "ACADAStore: Confirm your account", body);
                                            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                                            //ViewBag.Link = callbackUrl;
                                            ViewBag.IsEmailSent = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            //ViewBag.Link = "";
                                            ViewBag.IsEmailSent = false;
                                        }
                                        return View("DisplayEmail");
                                        #endregion
                                    }
                                    catch
                                    {
                                        txn.Rollback();
                                    }
                                }
                            }

                        }
                        else AddErrors(result);
                    }
                    AddErrors(result);
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    Logger.WriteLogFile("Registration", ex.Message, exceptionMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    // throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

            }
            model.UserTypes = CommonRepository.GetLookups(Enums.LookupType.UserType.ToString());
            model.ClasseTypes = CommonRepository.GetLookups(Enums.LookupType.ClassType.ToString());
            model.SubjectCategory = CommonRepository.GetLookups(Enums.LookupType.SubjectCategory.ToString()).DistinctBy(x => x.Value);
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public string CheckUserName(string input)
        {
            var user = UserManager.FindByEmail(input);
            if (user == null)
            {
                return "Available";
            }
            return "Not Available";
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                if (ConfirmAccount(userId))
                {
                    return View("ConfirmEmail");
                }
            }
            AddErrors(result);
            return View("Error");
        }
        private bool ConfirmAccount(string userId)
        {
            var context = new ApplicationDbContext();
            ApplicationUser user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Status = AppConstants.UserStatus.Active;
                DbSet<ApplicationUser> dbSet = context.Set<ApplicationUser>();
                dbSet.Attach(user);
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();

                return true;
            }
            return false;
        }
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.UserDoesNotExists = "We couldn't match the email you entered with information in our database. Please provide the correct email address.";
                    return View("ForgotPasswordConfirmation");
                }

                try
                {
                    string userName = "";
                    if (user.UserProfile != null)
                    {
                        userName = user.UserProfile.FullName;
                    }
                    var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    var body = EmailTemplates.GetEmailUser_ForgottenPassword(userName, callbackUrl);
                    await UserManager.SendEmailAsync(user.Id, "Reset Password", body);
                    ViewBag.IsEmailSent = true;
                }
                catch (Exception ex)
                {

                    //ViewBag.Link = "";
                    ViewBag.Email = "An email has not been sent to: " + model.Email + ". Please try again.";
                    ViewBag.IsEmailSent = false;
                    return View(model).WithError("An email has not been sent to: " + model.Email + ". Please try again.");
                }
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                //ViewBag.Link = callbackUrl;
                ViewBag.Email = "An email has been sent to: " + model.Email;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                AuthenticationManager.SignOut();
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                AuthenticationManager.SignOut();
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {

            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInHelper.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            // Generate the token and send it
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await SignInHelper.SendTwoFactorCode(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInHelper.ExternalSignIn(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                //     var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var user = new ApplicationUser
                {

                    UserType = "",
                    UserName = model.Email,
                    Email = model.Email,
                    ReferedBy = AppConstants.MyRMA,
                    Status = AppConstants.UserStatus.Created,
                    UserProfile = new UserProfile { FirstName = model.FirstName, LastName = model.LastName, DOB = model.DOB }
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        var roleName = AppConstants.Roles.Visitor;
                        UserManager.AddToRole(user.Id, roleName);
                        await SignInHelper.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> ResendEmailVerification(string UserId, string Code)
        {
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = UserId, code = Code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(UserId, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
            ViewBag.Link = callbackUrl;
            return View("DisplayEmail");
        }
        public ActionResult NotAuthorized()
        {
            //Response.StatusCode = 401;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Dashboard", "Account");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}