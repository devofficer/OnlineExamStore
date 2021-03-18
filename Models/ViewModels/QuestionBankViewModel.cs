using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace OnlineExam.Models
{
    public class QuestionBankIndexViewModel
    {
        public IPagedList<QuestionBankViewModel> QuestionBankList { get; set; }
        public QuestionBankSearchViewModel QuestionBankSearchViewModel { get; set; }
    }
    public class QuestionBankViewModel
    {
        public QuestionBankViewModel()
        {
            ClasseTypes = new HashSet<SelectListItem>();
            ExamTypes = new HashSet<SelectListItem>();
            Subjects = new HashSet<SelectListItem>();
            QuestionFormats = new HashSet<SelectListItem>();
        }
        public int Id { get; set; }

        [Display(Name = "Question Id")]
        public int QuestionId { get; set; }

        [Required]
        [Display(Name = "Question Decription:")]
        public string Decription { get; set; }
        public string ImagePath { get; set; }

        [Required]
        [Display(Name = "Option A:")]
        public string OptionA { get; set; }

        [Required]
        [Display(Name = "Option B:")]
        public string OptionB { get; set; }

        [Required]
        [Display(Name = "Option C:")]
        public string OptionC { get; set; }

        [Required]
        [Display(Name = "Option D:")]
        public string OptionD { get; set; }

        [Display(Name = "Option E:")]
        public string OptionE { get; set; }

        [Required]
        [Display(Name = "Answer Option")]
        public string AnswerOption { get; set; }

        [Required]
        [Display(Name = "Answer Explaination")]
        public string AnswerDescription { get; set; }


        public IEnumerable<SelectListItem> ClasseTypes { get; set; }
        public IEnumerable<SelectListItem> ExamTypes { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }
        public IEnumerable<SelectListItem> QuestionFormats { get; set; }

        [Required]
        [Display(Name = "Class Name", Prompt = "Select Class")]
        public string ClassName { get; set; }

        [Required]
        public string ExamName { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string QuestionFormat { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}