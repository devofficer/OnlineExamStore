using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class SystemQuestionRequiest
    {
        [Key]
        public int Id { get; set; }
        public int TeacherProfileId { get; set; }
        public int QuestionId { get; set; }
        public bool IsRequiest { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}