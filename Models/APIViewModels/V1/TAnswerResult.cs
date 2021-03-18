using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class TAnswerResult
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string AnswerOption { get; set; }
        public string Answer { get; set; }
        public string AnswerStatus { get; set; }
        public int Marks { get; set; }

        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string AnswerDescription { get; set; }
    }
}