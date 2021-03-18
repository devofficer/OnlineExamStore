using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class PostQuestionData
    {
        public int currentIndex { get; set; }
        public int preIndex { get; set; }
        public string formatType { get; set; }
        public string userAnswer { get; set; }

        public bool isNext { get; set; }
        public bool isSubmit { get; set; }

        public int duration { get; set; }

        public int attemptedDuration { get; set; }
        public List<AnswerViewModel> cQuestions { get; set; }

    }
}