using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class ReferralPragram
    {
        public ReferralPragram()
        {
            CreatedOn = DateTime.UtcNow;
        }
        /// <summary>
        /// Send Refferal Id in URL
        /// </summary>
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string ReferralLevelOne { get; set; }
        public string ReferralLevelTwo { get; set; }
        public string ReferralUrl { get; set; }
        public string ReferralCode { get; set; }
        /// <summary>
        /// InProgress/Redeemed, If status is "InProgress" then referral is pending at Admin for approval
        /// </summary>
        public string RedeemStatus { get; set; }
        public int ReferralOrderId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}