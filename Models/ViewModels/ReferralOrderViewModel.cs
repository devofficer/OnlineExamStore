using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class ReferralOrderViewModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
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
    }
}