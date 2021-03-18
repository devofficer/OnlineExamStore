using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public enum InstructionType
    {
        Section,
        General
    }
    public class Instruction
    {
        [Key]
        public int Id { get; set; }
        public InstructionType InstructionType { get; set; }
        public string RefNumber { get; set; } // Note: RefNumber should be start with prefix 'G'/'S'
        public string Description { get; set; }
    }
    public class Section
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class QuestionPaperMapping
    {
        [Key]
        public int Id { get; set; }

        public int QuestionBankId { get; set; }
        public int QuestionPaperId { get; set; }

        [ForeignKey("QuestionPaperId")]
        public QuestionPaper QuestionPaper { get; set; }

        [ForeignKey("QuestionBankId")]
        public QuestionBank QuestionBank { get; set; }

        public int SectionId { get; set; }

    }

    public class QuestionPaper
    {
        public QuestionPaper()
        {
            QuestionPaperMappings = new List<QuestionPaperMapping>();
        }

        [Key]
        public int Id { get; set; }
        //public int Hour { get; set; }
        public int? Minute { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }
        public string ClassName { get; set; }
        public string ExamName { get; set; }
        public string Subject { get; set; }

        public List<QuestionPaperMapping> QuestionPaperMappings { get; set; }

        public bool IsTrial { get; set; }
        public bool IsOnline { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}