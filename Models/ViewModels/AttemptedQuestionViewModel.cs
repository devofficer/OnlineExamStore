using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class AttemptedQuestionViewModel
    {
        public AttemptedQuestionViewModel()
        {
            CreatedOn = DateTime.Now;    
        }
        public int Id { get; set; }
        public int QuestionPaparId { get; set; }

        public int QuestionId { get; set; }

        public string SelectedAnsOption { get; set; }
        public bool IsCorrectAnswer { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}