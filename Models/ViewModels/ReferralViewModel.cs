using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class PostReferralViewModel
    {
        public int Id { get; set; }
        public bool IsPaidByBank { get; set; }
        public decimal Bonus { get; set; }
    }
    public class ReferralViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ReferralProgramId { get; set; }
        public string RefererEmail { get; set; }

        [Display(Name="Demo")]
        public bool IsDemo { get; set; }

        [Display(Name = "Paid(Voucher)")]
        public bool IsPaidByVoucher { get; set; }

        [Display(Name = "Paid(Bank)")]
        public bool IsPaidByBank { get; set; }

        [Display(Name = "Bonus(₦)")]
        public double Bonus { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}