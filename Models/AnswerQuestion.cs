using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OnlineExam.Models
{
    public class AnswerQuestion
    {
        [Key]
        public int Id { get; set; }

        public int QuestionPaperId { get; set; }
        [ForeignKey("QuestionPaperId")]
        public QuestionPaper QuestionPaper { get; set; }

        public int QuestionBankId { get; set; }
        //[ForeignKey("QuestionBankId")]
        //public QuestionBank QuestionBank { get; set; }

        public string UserId { get; set; }

        public int UserMaxId { get; set; }

        public int? ParentId { get; set; }
        public string Answer { get; set; }

        public int TimeTakenInSecond { get; set; }
        public string FormatType { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
