using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class ReferralOrder
    {
        public ReferralOrder()
        {
            CreatedOn = DateTime.UtcNow;
        }
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 1,2,3,4,--- inculding bank paid and voucher paid
        /// </summary>
        public string ReferralPragramIds { get; set; }

        /// <summary>
        /// ReferralType: 1stLevel/2ndLevel
        /// </summary>
        public string ReferralType { get; set; }
        /// <summary>
        /// Total Bank paid referrals
        /// </summary>
        public int ReferralClaimCount { get; set; }
        /// <summary>
        /// Calculated bonus for bank paid referrals
        /// </summary>
        public decimal TotalBonus { get; set; }
        /// <summary>
        /// PaymentStatus : Paid/Pending
        /// </summary>
        public string PaymentStatus { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}