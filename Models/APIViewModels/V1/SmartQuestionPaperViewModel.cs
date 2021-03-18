using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class SmartQuestionPaperViewModel
    {
        public SmartQuestionPaperViewModel()
        {
            Questions = new List<SmartQuestionViewModel>();
        }
        public int Id { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }

        public string Subject { get; set; }
        public int QuestionCount { get; set; }
        public int TotalMarks { get; set; }
        public string UserId { get; set; }

        public List<SmartQuestionViewModel> Questions { get; set; }
    }
}