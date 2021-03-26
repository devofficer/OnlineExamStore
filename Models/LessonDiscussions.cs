using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class LessonDiscussions
    {
        [Key]
        public long Id { get; set; }
        public int LessonId { get; set; }
        public int ProfileId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}