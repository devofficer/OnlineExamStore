using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class SmartQuestionViewModel
    {
        public SmartQuestionViewModel()
        {
            ChildQuestions = new List<SmartQuestionViewModel>();
        }
        public int Index { get; set; }
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string UserAnswer { get; set; }

        public bool IsAttempted { get; set; }

        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string FormatType { get; set; }
        public string ImagePath { get; set; }
        public string CreatedBy { get; set; }

        public List<SmartQuestionViewModel> ChildQuestions { get; set; }
    }
}