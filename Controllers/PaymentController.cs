using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Models;
using OnlineExam.Models.ViewModels;
using OnlineExam.Repositories;
using PagedList;
using Paystack.Net.SDK.Transactions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private ApplicationDbContext _dbContext = new ApplicationDbContext();
        // GET: Payment
        public ActionResult Index(int? page)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var info = _dbContext.UserPayments.Where(x => x.ProfileId == profileId).ToList()
                .Select(x => new PaymentViewModel
                {
                    Id = x.Id,
                    Date = x.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    PaymentType = x.PaymentType,
                    PaymentMethod = x.PaymentMethod,
                    TransactionId = x.TransactionId,
                    TransactionMessage = x.TransactionMessage,
                    TransactionStatus = x.TransactionStatus,
                    Bank = x.Bank,
                    TxnId = x.TxnId,
                    ProofFile = x.ProofFile,
                    IsApproved = x.IsApproved == true?"Yes":"No",
                    Amount = x.Amount
                });

            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            ViewBag.AccountBalance = "0.00";

            if(_dbContext.UserAccountBalance.Where(x=>x.ProfileId == profileId).Count() > 0)
            {
                ViewBag.AccountBalance = _dbContext.UserAccountBalance.FirstOrDefault(x => x.ProfileId == profileId).Amount.ToString("0.00");
            }


            return View(info.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult MakePayment()
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();

            ViewBag.AccountBalance = "0.00";

            if (_dbContext.UserAccountBalance.Where(x => x.ProfileId == profileId).Count() > 0)
            {
                var info = _dbContext.UserAccountBalance.FirstOrDefault(x => x.ProfileId == profileId);
                ViewBag.AccountBalance = info.Amount.ToString("0.00");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MakePayment(UserPayments model, HttpPostedFileBase Proof)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            var userInfo = _dbContext.UserProfiles.Find(profileId);
            Guid transactionGuId = Guid.NewGuid();

            if (model.PaymentType == "Online")
            {
                model.PaymentGuId = transactionGuId;
                model.ProfileId = profileId;
                model.Date = DateTime.Now;
                model.PaymentMethod = "PayStack";
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.IsApproved = false;

                _dbContext.UserPayments.Add(model);
                _dbContext.SaveChanges();

                

                string secretKey = ConfigurationManager.AppSettings["PaystackSecret"];
                var paystackTransactionAPI = new PaystackTransaction(secretKey);
                var response = await paystackTransactionAPI.InitializeTransaction(userInfo.Email, (((int)model.Amount) * 100), userInfo.FirstName, userInfo.LastName, "http://localhost:41434/Payment/CallBack/" + transactionGuId, profileId.ToString());
                // var response = await paystackTransactionAPI.InitializeTransaction(userInfo.Email, (int)model.Amount, userInfo.FirstName, userInfo.LastName);


                // Response.Write(response.message + " -- " + response.status + " -- " + response.data.access_code + " -- " + response.data.authorization_url);
                // Response.End();
                return Redirect(response.data.authorization_url);
                
            }
            else
            {
                string filePath = "~/FileUploads/PaymentFroof";

                bool folderExists = Directory.Exists(Server.MapPath(filePath));
                if (!folderExists)
                    Directory.CreateDirectory(Server.MapPath(filePath));

                if (string.IsNullOrEmpty(model.Bank))
                    return RedirectToAction("MakePayment").WithError("Bank cannot be empty!");
                if (Proof==null)
                    return RedirectToAction("MakePayment").WithError("Proof file cannot be empty!");

                if (!string.IsNullOrEmpty(model.Bank) && Proof != null)
                {
                    var fileName = Path.GetFileName(Proof.FileName);
                    var type = Path.GetExtension(Proof.FileName);

                    byte[] fileBytes = new byte[Proof.ContentLength];
                    Proof.InputStream.Read(fileBytes, 0, Proof.ContentLength);

                    string newFileName = transactionGuId.ToString() + type;

                    var fullPath = Path.Combine(Server.MapPath(filePath + "/"), newFileName);

                    Proof.SaveAs(fullPath);


                    model.PaymentGuId = transactionGuId;
                    model.ProfileId = profileId;
                    model.Date = DateTime.Now;
                    model.PaymentMethod = "Bank Transfer";
                    model.ProofFile = newFileName;
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.IsApproved = false;

                    _dbContext.UserPayments.Add(model);
                    _dbContext.SaveChanges();

                    return RedirectToAction("MakePayment").WithSuccess("Payment details successfully updated! Admin has review and will approve soon");

                }

                    return View();
            }
        }

        public async Task<ActionResult> CallBack(Guid id)
        {
            int profileId = CommonRepository.GetCurrentUserProfileId();
            string secretKey = ConfigurationManager.AppSettings["PaystackSecret"];
            var paystackTransactionAPI = new PaystackTransaction(secretKey);
            var tranxRef = HttpContext.Request.QueryString["reference"];
            if (tranxRef != null)
            {
                var response = await paystackTransactionAPI.VerifyTransaction(tranxRef);
                // Response.Write(response.data.customer.email);
                // Response.End();
                var info = _dbContext.UserPayments.FirstOrDefault(x => x.PaymentGuId == id);
                if (response.status==true)
                {
                    info.TransactionId = tranxRef;
                    info.TransactionMessage = response.data.gateway_response;
                    info.TransactionStatus = response.data.status;
                    info.Bank = response.data.authorization.bank;
                    info.IsApproved = true;
                    info.ApprovedDate = DateTime.Now;
                    info.ApprovedUserProfileId = profileId;

                    _dbContext.SaveChanges();

                    decimal PaidAmount = info.Amount;

                    if (_dbContext.UserAccountBalance.Where(x => x.ProfileId == profileId).Count() == 0)
                    {
                        var paymentInfo = new UserAccountBalance();
                        paymentInfo.ProfileId = profileId;
                        paymentInfo.Amount = PaidAmount;
                        paymentInfo.ModifiedDate = DateTime.Now;

                        _dbContext.UserAccountBalance.Add(paymentInfo);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        var paymentInfo = _dbContext.UserAccountBalance.FirstOrDefault(x=>x.ProfileId == profileId);
                        paymentInfo.Amount += PaidAmount;

                        _dbContext.SaveChanges();
                    }


                    return RedirectToAction("MakePayment").WithSuccess("Payment details successfully updated!");
                }
                else
                {
                    info.TransactionId = tranxRef;
                    info.TransactionMessage = response.data.message.ToString();
                    info.TransactionStatus = response.data.status;

                    _dbContext.SaveChanges();

                    return RedirectToAction("MakePayment").WithError("Payment failed! " + response.message.ToString());
                }
            }

            return RedirectToAction("MakePayment").WithError("Payment failed!");
        }

        [Authorize(Roles = "StaffAdmin")]
        public ActionResult PendingApproval(int? page)
        {
           // int profileId = CommonRepository.GetCurrentUserProfileId();
            var info = _dbContext.UserPayments.Where(x => x.PaymentType == "Offline" && x.IsApproved == false).ToList()
                .Select(x => new PaymentViewModel
                {
                    Id = x.Id,
                    UserName = _dbContext.UserProfiles.Find(x.ProfileId).FullName,
                    ClassType = _dbContext.UserProfiles.Find(x.ProfileId).ClassTypes,
                    Date = x.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    PaymentType = x.PaymentType,
                    PaymentMethod = x.PaymentMethod,
                    TransactionId = x.TransactionId,
                    TransactionMessage = x.TransactionMessage,
                    TransactionStatus = x.TransactionStatus,
                    Bank = x.Bank,
                    TxnId = x.TxnId,
                    ProofFile = x.ProofFile,
                    IsApproved = x.IsApproved == true ? "Yes" : "No",
                    Amount = x.Amount
                });

            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);

            
            return View(info.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "StaffAdmin")]
        public ActionResult PaymentReview(long id)
        {
            var info = _dbContext.UserPayments.Find(id);

            var userInfo = _dbContext.UserProfiles.Find(info.ProfileId);
            ViewBag.ClassType = userInfo.ClassTypes;
            ViewBag.FullName = userInfo.FullName;

            return View(info);
        }

        [HttpPost]
        public ActionResult PaymentReview(long id, UserPayments model)
        {
            try
            {
                int profileId = CommonRepository.GetCurrentUserProfileId();
                var paymentInfo = _dbContext.UserPayments.Find(id);
                bool status = paymentInfo.IsApproved;
                paymentInfo.ModifiedDate = DateTime.Now;
                paymentInfo.IsApproved = model.IsApproved;
                paymentInfo.TransactionStatus = model.TransactionStatus;
                paymentInfo.TransactionMessage = model.TransactionMessage;
                paymentInfo.Note = model.Note;
                if (model.IsApproved == true)
                {
                    paymentInfo.ApprovedDate = DateTime.Now;
                    paymentInfo.ApprovedUserProfileId = profileId;
                }
                _dbContext.SaveChanges();

                if (model.IsApproved == true && status == false)
                {
                    if (_dbContext.UserAccountBalance.Where(x => x.ProfileId == paymentInfo.ProfileId).Count() == 0)
                    {
                        var balanceInfo = new UserAccountBalance();
                        balanceInfo.ProfileId = paymentInfo.ProfileId;
                        balanceInfo.Amount = paymentInfo.Amount;
                        balanceInfo.ModifiedDate = DateTime.Now;

                        _dbContext.UserAccountBalance.Add(balanceInfo);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        var balanceInfo = _dbContext.UserAccountBalance.FirstOrDefault(x => x.ProfileId == paymentInfo.ProfileId);
                        balanceInfo.Amount += paymentInfo.Amount;
                        balanceInfo.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();
                    }

                }
                return RedirectToAction("PendingApproval").WithSuccess("Payment info updated! ");
            }
            catch
            {
                var info = _dbContext.UserPayments.Find(id);

                var userInfo = _dbContext.UserProfiles.Find(info.ProfileId);
                ViewBag.ClassType = userInfo.ClassTypes;
                ViewBag.FullName = userInfo.FullName;

                return RedirectToAction("PaymentReview", new {id=id }).WithError("Payment info update failed! ");
            }
        }

        [Authorize(Roles = "StaffAdmin")]
        public ActionResult DownLoad(string strfile)
        {
            string filePath = "~/FileUploads/PaymentFroof/";

            string strPath = Path.Combine(Server.MapPath(filePath), strfile); ;
            byte[] fileBytes = System.IO.File.ReadAllBytes(strPath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, strfile);
        }

        [Authorize(Roles = "StaffAdmin")]
        public ActionResult StudentPayments(int? page)
        {
            // int profileId = CommonRepository.GetCurrentUserProfileId();
            var info = _dbContext.UserPayments.Where(x => x.PaymentType == "Offline" && x.IsApproved == false).ToList()
                .Select(x => new PaymentViewModel
                {
                    Id = x.Id,
                    UserName = _dbContext.UserProfiles.Find(x.ProfileId).FullName,
                    ClassType = _dbContext.UserProfiles.Find(x.ProfileId).ClassTypes,
                    Date = x.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    PaymentType = x.PaymentType,
                    PaymentMethod = x.PaymentMethod,
                    TransactionId = x.TransactionId,
                    TransactionMessage = x.TransactionMessage,
                    TransactionStatus = x.TransactionStatus,
                    Bank = x.Bank,
                    TxnId = x.TxnId,
                    ProofFile = x.ProofFile,
                    IsApproved = x.IsApproved == true ? "Yes" : "No",
                    Amount = x.Amount
                });

            info = info.OrderByDescending(x => x.Id);
            int pageSize = 20;

            int pageNumber = (page ?? 1);


            return View(info.ToPagedList(pageNumber, pageSize));
        }
    }
}