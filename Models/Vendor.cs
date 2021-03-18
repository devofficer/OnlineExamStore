using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Vendor
    {
        public Vendor()
        {
            Vouchers = new List<Voucher>();
        }

        [Key]
        public int Id { get; set; }

        public List<Voucher> Vouchers { get; set; }

        [Required]
        [Display(Name = "Vendor Code")] // AUTO GENERATED 5 DIGITS CODE
        public string VendorCode { get; set; }
        [Required]
        public string Name { get; set; }

        public string Avatar { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        // ADDRESS
        [Display(Name = "Address Line1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        // CONTACT DETAIL
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Contact Detail")]
        public string ContactDetail { get; set; }
        [Required]
        [Display(Name = "Primary Email")]
        public string PrimaryEmail { get; set; }

        [Display(Name = "Secondary Email")]
        public string SecondaryEmail { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}