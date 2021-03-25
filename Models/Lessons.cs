using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Lessons
    {
        [Key]
        public int LessonId { get; set; }
        public Guid LessionGuId { get; set; }
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Class Type is required")]
        public string ClassType { get; set; }
        [Required(ErrorMessage = "Subject Category is required")]
        public string SubjectCategory { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}