using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;
using Newtonsoft.Json;

namespace OnlineExam.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            SecondaryContactNo = string.Empty;
            States = new List<SelectListItem>();
            Cities = new List<SelectListItem>();
            ClassTypes = new List<SelectListItem>();
            Schools = new List<SelectListItem>();
          
            
        }
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        //public string Password { get; set; }

        public string UserType { get; set; }

        [Required]
        public DateTime DOB { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }

        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        [Display(Name = "Class Name")]
        public string[] SelectedClasses { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Display(Name = "Is Correction Required?")]
        public bool IsCorrectionRequired { get; set; }

        public bool IsBankDetailReadOnly { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Referrer Email")]
        public string ReferrerEmail { get; set; }

        public string CountryId { get; set; }
        [Display(Name = "Country")]
        public List<SelectListItem> Countries { get; set; }

        [Display(Name = "State")]
        public List<SelectListItem> States { get; set; }

        [Display(Name = "City")]
        public List<SelectListItem> Cities { get; set; }

        [Display(Name = "School")]
        public List<SelectListItem> Schools { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Primary Contact")]
        //[Required]
        [DataType(DataType.PhoneNumber)]
        public string PrimaryContactNo { get; set; }

        [DataType(DataType.PhoneNumber), Display(Name = "Mobile"), Required(ErrorMessage = "Mobile field is requird.")]
        public string SecondaryContactNo { get; set; }

        public string RoleId { get; set; }
        [Display(Name = "Role")]
        public List<SelectListItem> Roles { get; set; }

        [Display(Name = "School Name"), Required]
        public string SchoolName { get; set; }

        [Display(Name = "Others"), Required]
        public string OthersName { get; set; }

        [Display(Name = "School Address"), DataType(DataType.MultilineText)]
        public string SchoolAddress { get; set; }
        public string Hobbies { get; set; }
        public string Avatar { get; set; }
        //public int CompanyId { get; set; }
        //[Display(Name = "Company")]
        public List<SelectListItem> ClassTypes { get; set; }
        public bool IsMyProfile { get; set; }

    }
}