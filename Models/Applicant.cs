using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineExam.Utils;

namespace OnlineExam.Areas.Visa.Models
{
    public class Applicant
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Citizenship { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int ZipCode { get; set; }
    }
}