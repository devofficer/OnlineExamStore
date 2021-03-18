using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Models
{
    public class CompanyViewModel
    {
        public CompanyViewModel()
        {
            Documents = new List<Document>();
            //Users = new HashSet<ApplicationUser>();
            //Messages = new HashSet<Message>();
        }
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }
        [Required]
        public string Name { get; set; }

        public byte[] Logo { get; set; }
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

        // COMPANY DETAIL

        [Display(Name = "TIN Number")]
        public string TinNumber { get; set; }

        [Display(Name = "Service Number")]
        public string ServiceNumber { get; set; }

        [Display(Name = "Other Number")]
        public string OtherNumber { get; set; }

        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        [Display(Name = "Relationship Status")]
        public string RelationshipStatus { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public string Status { get; set; }

        public string CreatedBy { get; set; }
        public bool IsSelfAdmin { get; set; }      
        // ONE COMPANY WILL HAVE ONE DOCUMENT
        public List<Document> Documents { get; set; }
        //public ICollection<ApplicationUser> Users { get; set; }
        //public ICollection<Message> Messages { get; set; }

    }
}