using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class TestResultViewModel
    {
        public TestResultViewModel()
        {
            TestResultQuestionViewModel = new List<TestResultQuestionViewModel>();
        }
        public int QuestionPaparId { get; set; }
        public string QuestionPaparName { get; set; }
        public int TimeTakenInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCorrectedQuestions { get; set; }
        public int TotalInCorrectQuestions { get; set; }
        public int TotalAttemptedQuestions { get; set; }
        public int TotalUnAttemptedQuestions { get; set; }
        public string Status { get; set; }
        public string FormatType { get; set; }
        public List<TestResultQuestionViewModel> TestResultQuestionViewModel { get; set; }

    }

    public class TestResultQuestionViewModel
    {
        public TestResultQuestionViewModel()
        {
            ChildQuestions = new List<TestResultQuestionViewModel>();
        }
        public int QuestionId { get; set; }
        public int AttemptedQuestionPaparId { get; set; }
        public string SelectedAnsOption { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string AnswerOption { get; set; }
        public string AnswerDescription { get; set; }
        public string ParentQuestion { get; set; }
        public int Mark { get; set; }
        public int IsOnline { get; set; }
        public List<TestResultQuestionViewModel> ChildQuestions { get; set; }
    }
    public class QuestionViewModel
    {
        public QuestionViewModel()
        {
            PageIndex = 1;
            IsPreButtonEnabled = false;
            IsNxtButtonEnabled = true;
            ChildQuestions = new List<QuestionViewModel>();
        }
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public string Command { get; set; }
        public int PageIndex { get; set; }
        public int TotalQuestion { get; set; }
        public int QuestionCount { get; set; }

        public int AttemptedQuestionId { get; set; }
        public int AttemptedQuestionPaparId { get; set; }

        public int QuestionPaparId { get; set; }
        public int Minute { get; set; }
        public string QuestionPaparName { get; set; }
        public string Description { get; set; }
        public bool IsNxtButtonEnabled { get; set; }
        public bool IsPreButtonEnabled { get; set; }
        public string SelectedOptionA { get; set; }

        public string AnswerOption { get; set; }
        public int Mark { get; set; }
        public int TotalMarks { get; set; }

        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string FormatType { get; set; }
        public string ImagePath { get; set; }

        public List<QuestionViewModel> ChildQuestions { get; set; }
    }
}