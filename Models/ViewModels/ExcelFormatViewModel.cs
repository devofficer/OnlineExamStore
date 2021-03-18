using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class ExcelFormatViewModel
    {
        public string Class { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string FormatType { get; set; }

        public int PassageInstructionID { get; set; }
        public string PassageText { get; set; }
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }

        public string AnswerOptionA { get; set; }
        public string AnswerOptionB { get; set; }
        public string AnswerOptionC { get; set; }
        public string AnswerOptionD { get; set; }

        public string AnswerOptionE { get; set; }

        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public int Marks { get; set; }
        public string ImagePath { get; set; }
        public int Time { get; set; }
        public string Topic { get; set; }
     
    }
    public class ExcelFormat1ViewModel
    {
        public string Class { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string FormatType { get; set; }

        //public int PassageInstructionID { get; set; }
        //public string PassageText { get; set; }
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }

        public string AnswerOptionA { get; set; }
        public string AnswerOptionB { get; set; }
        public string AnswerOptionC { get; set; }
        public string AnswerOptionD { get; set; }
        public string AnswerOptionE { get; set; }

        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public int Marks { get; set; }
        public string ImagePath { get; set; }
        public int Time { get; set; }

    }
}