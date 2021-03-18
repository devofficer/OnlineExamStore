using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }
        //[Required]
        [Display(Name = "System Code")] // VENDOR CODE + DENOMINATION + AUTO GENERATED 5 DIGITS CODE
        public string SystemCode { get; set; }

        [Display(Name = "Date Of Issue")]
        public DateTime DateOfIssue { get; set; }

        [Display(Name = "Date Of Expiry")]
        public DateTime? DateOfExpiry { get; set; }

        //[Required]
        [Display(Name = "Voucher Code")]
        public string VoucherCode { get; set; }

        public int Denomination { get; set; }

        //[ForeignKey("VendorId")]
        //public Vendor Vendor { get; set; }
        public int VendorId { get; set; }

        public int MembershipPlanId { get; set; }

       
        //[ForeignKey("MembershipPlanId")]
        //public MembershipPlan MembershipPlan { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}