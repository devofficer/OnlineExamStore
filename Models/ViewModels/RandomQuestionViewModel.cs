using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class RandomQuestionViewModel
    {
        public int QuestionId { get; set; }
        public int DurationInSecond { get; set; }
        public string CreatedBy { get; set; }
    }
}