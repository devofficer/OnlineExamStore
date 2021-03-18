using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class ReferralPragramViewModel
    {
        public string RefferarUserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string ReferralLevelOne { get; set; }
        public string ReferralLevelTwo { get; set; }
        public string ReferralUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}