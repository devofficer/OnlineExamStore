using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class AttemptedQuestionPaparViewModel
    {
        public AttemptedQuestionPaparViewModel()
        {
            CreatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string UserId { get; set; }

        public int QuestionPaparId { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public bool IsCompletelyAttempted { get; set; }
        public int TotalCorrectedAnswered { get; set; }
        public int TotalInCorrectedAnswered { get; set; }

        public int Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        //public PagedList.IPagedList<QuestionListViewModel> Questions { get; set; }
        public PagedList.IPagedList<AttemptedQuestionViewModel> AttemptedQuestions { get; set; }
    }
}