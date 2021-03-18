using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Infrastructure;
using OnlineExam.Repositories;
using OnlineExam.Utils;

namespace OnlineExam.Models.ViewModels
{
    public class VoucherViewModel
    {
        public VoucherViewModel()
        {
            NoOfVoucher = 1;
            Vouchers = new List<Voucher>();
            Denominations = CommonRepository.GetLookups(Enums.LookupType.DenominationType.ToString()); //new List<int> { 10, 20, 30, 50, 100, 150, 200 };
            MembershipPlans = CommonRepository.GetMemberships();
            Vendors = CommonRepository.GetVendors();
        }

        public int Id { get; set; }

        // VENDOR CODE + DENOMINATION + AUTO GENERATED 5 DIGITS CODE
        //[Required]
        public string SystemCode { get; set; }

        [Display(Name = "Date Of Issue")]
        public DateTime DateOfIssue { get; set; }

        [Display(Name = "Date Of Expiry")]
        public DateTime? DateOfExpiry { get; set; }

        //[Required]
        [Display(Name = "Voucher Code")]
        public string VoucherCode { get; set; }

        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Display(Name = "# Of Voucher")]
        [Required]
        public int NoOfVoucher { get; set; }


        [Display(Name = "Denomination")]
        public int SelectedDenomination { get; set; }

        [Display(Name = "Vendor")]
        public int SelectedVendorId { get; set; }

        [Display(Name = "Membership")]
        public int SelectedMembershipPlanId { get; set; }

        public List<Voucher> Vouchers { get; set; }
        public IEnumerable<SelectListItem> Denominations { get; set; }
        public IEnumerable<SelectListItem> MembershipPlans { get; set; }
        public IEnumerable<SelectListItem> Vendors { get; set; }

        
        public int Status { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}