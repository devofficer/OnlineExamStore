using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Web.Mvc;
using OnlineExam.Utils;

namespace OnlineExam.Models
{
    public class ProductSelectionViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public bool IsReceiptAlreadyCreated { get; set; }
        public List<SelectProductEditorViewModel> Product { get; set; }
        public ProductSelectionViewModel()
        {
            this.Product = new List<SelectProductEditorViewModel>();
            this.Invoices = new List<Invoice>();
        }
        public IEnumerable<int> GetSelectedIds()
        {
            // Return an Enumerable containing the Id's of the selected people:
            return (from p in this.Product where p.Selected select p.ProductId).ToList();
        }
        public List<Invoice> Invoices { get; set; }

    }
    public class SelectProductEditorViewModel
    {
        public bool Selected { get; set; }

        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal? Tax { get; set; }
        public decimal? LineTotal { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required]

        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class UserPlanViewModel
    {
        public UserPlanViewModel()
        {
            IsBankPaymentOption = true;
            Despositor = "43567";
            Narration = "43567";
            Beneficiary = "ACADASTORE.COM";
            Bank = "CANARA BANK";
            Account = "BA676464746";
            Amount = 1500;
        }
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Voucher Code")]
        [StringLength(12, ErrorMessage = "Must be between {2} and {1} characters long.", MinimumLength = 6)]
        public string VoucherCode { get; set; }

        public int MembershipPlanId { get; set; }

        public int VoucherId { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        //HAVE ADDED ON 5TH APRIL 2017
        // TO ADD BANK PAYMENT OPTION

        public bool IsVoucherPaymentOption { get; set; }
        public bool IsBankPaymentOption { get; set; }
        public bool IsOtherPaymentOption { get; set; }
        /// <summary>
        /// Txn Id should be BANK Voucher code which will display on UI as DEPOSITOR & NARRATION
        /// </summary>
        public string TxnId { get; set; }
        public double Amount { get; set; }
        public string Despositor { get; set; }
        public string Narration { get; set; }
        public string Beneficiary { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public OnlineExam.Utils.PaymentStatus PaymentStatus { get; set; }
    }
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            ClasseTypes = new HashSet<SelectListItem>();
        }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Voucher Code")]
        public string VoucherCode { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public string RegisterType { get; set; }
        public string ReferralCode { get; set; }

        [Display(Name="Referrer Email")]
        public string ReferrerEmail { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        public string UserType { get; set; }

        [Display(Name = "User Type")]
        public IEnumerable<SelectListItem> UserTypes { get; set; }

        public string[] SubjectCategories { get; set; }

        [Display(Name = "Category")]
        public IEnumerable<SelectListItem> SubjectCategory { get; set; }

        public string RoleId { get; set; }
        [Display(Name = "Role")]
        public List<SelectListItem> Roles { get; set; }

        [Display(Name = "Select Class")]
        public string[] SelectedClasses { get; set; }
        public IEnumerable<SelectListItem> ClasseTypes { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}