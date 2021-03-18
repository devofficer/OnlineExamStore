using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class QuestionError
    {
        public QuestionError()
        {
            CreatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string ReportType { get; set; }
        public string ActionTaken { get; set; } //(Status: Ignored/Corrected/Submitted)
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class QuestionErrorViewModel
    {
        public QuestionErrorViewModel()
        {
            CreatedOn = DateTime.Now;
        }

        public int Index { get; set; }
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }

        public string ActionTaken { get; set; } //(Status: Ignored/Corrected/Submitted)
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}