using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class UserPlan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }

        [ForeignKey("MembershipPlanId")]
        public MembershipPlan MembershipPlan { get; set; }
        
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
    }
}