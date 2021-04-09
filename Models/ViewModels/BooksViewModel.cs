using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class BooksViewModel
    {
        public int Id { get; set; }
        public string ClassType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }

    public class BooksAssignViewModel
    {
        public int Id { get; set; }
        public Guid AssignGuId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int BookId { get; set; }
        public string ClassType { get; set; }
        public string BookTitle { get; set; }
        public string TimeFrame { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime dtEndDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }

    public class BooksReviewViewModel
    {
        public long Id { get; set; }
        public int AssignId { get; set; }
        public int ProfileId { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
        public string UserAvater { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }
}