using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
}