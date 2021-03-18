using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Value { get; set; }
        public bool IsRightOption { get; set; }

    }
}