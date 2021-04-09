using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class BooksAssign
    {
        [Key]
        public int Id { get; set; }
        public Guid AssignGuId { get; set; }
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Book is required")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "Time Frame is required")]
        public string TimeFrame { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}