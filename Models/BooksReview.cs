using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class BooksReview
    {
        [Key]
        public long Id { get; set; }
        public int AssignId { get; set; }
        public int ProfileId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}