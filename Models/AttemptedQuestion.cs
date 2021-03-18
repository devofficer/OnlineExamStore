using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class AttemptedQuestion
    {
        public AttemptedQuestion()
        {
            CreatedOn = DateTime.Now;    
        }
        [Key]
        public int Id { get; set; }

        [ForeignKey("AttemptedQuestionPaparId")]
        public AttemptedQuestionPapar AttemptedQuestionPapar { get; set; }
        public int AttemptedQuestionPaparId { get; set; }

        //[ForeignKey("QuestionId")]
        //public QuestionBank QuestionBank { get; set; }
        public int QuestionId { get; set; }

        public int Mark { get; set; }

        public string AnswerOption { get; set; }
        public string SelectedAnsOption { get; set; }
        public bool IsCorrectAnswer { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int? ParentId { get; set; }
    }
}