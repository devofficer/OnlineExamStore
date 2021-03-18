using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class TResult
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationInMinutes { get; set; }
        public int? TimeTakenInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttemptedQuestions { get; set; }
        public int TotalCorrectedAnswers { get; set; }
        public int TotalInCorrectedAnswers { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public bool IsCompletelyAttempted { get; set; }
        public string FinalStatus { get; set; }
        public string UserId { get; set; }
    }
}