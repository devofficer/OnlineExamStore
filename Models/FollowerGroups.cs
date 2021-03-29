using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class FollowerGroups
    {
        [Key]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}