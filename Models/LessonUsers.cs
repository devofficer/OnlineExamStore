using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class LessonUsers
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int AttendUserProfileId { get; set; }
        public DateTime AttendDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}