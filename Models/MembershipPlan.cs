using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class MembershipPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Membership Plan Code")] // AUTO GENERATED 5 DIGITS CODE
        public string MembershipPlanCode { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Validity In Days")]
        public int ValidityInDays { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}