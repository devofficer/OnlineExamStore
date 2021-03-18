using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class QuizViewModel
    {
        public int SectionId { get; set; }
        public int QuestionId { get; set; }
        public string UserOption { get; set; }
        public string Format { get; set; }
        public Option Option { get; set; }
    }
}