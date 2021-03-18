using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class AttemptedQuestionPapar
    {
        public AttemptedQuestionPapar()
        {
            CreatedOn = DateTime.Now;
            AttemptedQuestions = new List<AttemptedQuestion>();
        }
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }

        [ForeignKey("QuestionPaparId")]
        public QuestionPaper QuestionPaper { get; set; }
        public int QuestionPaparId { get; set; }

        public List<AttemptedQuestion> AttemptedQuestions { get; set; }

        public string QuestionPaparName { get; set; }
        public int Duration { get; set; }
        public int TimeTakenInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public bool IsCompletelyAttempted { get; set; }
        public int TotalCorrectedAnswered { get; set; }
        public int TotalInCorrectedAnswered { get; set; }

        public string FormatType { get; set; }

        public bool IsArchive { get; set; }

        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}