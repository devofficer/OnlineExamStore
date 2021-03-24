using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class LessonNotes
    {
        [Key]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string ClassCategory { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string PaymentType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}