using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class School
    {
        public School()
        {
            Banners = new List<Banner>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
        public string Status { get; set; } //Created by default, Admin staff will approved it Approved
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Action { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public List<Banner> Banners { get; set; }
    }

    public class Banner
    {
        public int Id { get; set; }

        [ForeignKey("SchoolId")]
        public School School { get; set; }
        public int SchoolId { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class SchoolNews
    {
        public int Id { get; set; }

        [ForeignKey("SchoolId")]
        public School School { get; set; }
        public int SchoolId { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}