using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TeacherViewModel
    {
        public int UserProfileId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string RegisterdDate { get; set; }
        public string Follow { get; set; }

    }
}