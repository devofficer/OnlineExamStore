using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class QuestionViewModelV2
    {
        public QuestionViewModelV2()
        {
            ChildQuestions = new List<QuestionViewModelV2>();
        }
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int QuestionPaperId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string UserOption { get; set; }
        public string AnswerOption { get; set; }
        public string Solution { get; set; }
        public int Mark { get; set; }
        public int Duration { get; set; }
        public bool IsOnline { get; set; }
        public int ParentId { get; set; }
        public string Subject { get; set; }
        public string FormatType { get; set; }
        public string ImagePath { get; set; }

        public List<QuestionViewModelV2> ChildQuestions { get; set; }
    }
    public class ChildQuestionViewModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ParentId { get; set; }
        public int QuestionPaperId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string UserOption { get; set; }
        public string Solution { get; set; }
        public string FormatType { get; set; }
        public string ImagePath { get; set; }
        public int Mark { get; set; }
        public int Duration { get; set; }
        //public string Subject { get; set; }
    }
}