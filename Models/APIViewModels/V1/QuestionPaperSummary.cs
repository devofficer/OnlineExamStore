using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class QuestionPaperSummary
    {
        public int QuestionPaperId { get; set; }
        public string Name { get; set; }
        public int TotalMarks { get; set; }
        public int Duration { get; set; }
        public int QuestionCount { get; set; }

        public string Subject { get; set; }
    }
}