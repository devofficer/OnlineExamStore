using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace OnlineExam.Models
{
    public class QuestionPaperIndexViewModel
    {
        public QuestionPaperIndexViewModel()
        {
            QuestionBankSearchViewModel = new QuestionBankSearchViewModel();
            QuestionPaperList = new List<QuestionPaperViewModel>();
            QuestionPaperMappings = new List<QuestionPaperMapping>();
            Minutes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "5", Value = "5" },
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "15", Value = "15" },
                    new SelectListItem { Text = "20", Value = "20" },
                    new SelectListItem { Text = "25", Value = "25" },
                    new SelectListItem { Text = "30", Value = "30" },
                    new SelectListItem { Text = "35", Value = "35" },
                    new SelectListItem { Text = "40", Value = "40" },
                    new SelectListItem { Text = "45", Value = "45" },
                    new SelectListItem { Text = "50", Value = "50" },
                    new SelectListItem { Text = "55", Value = "55" },
                    new SelectListItem { Text = "60", Value = "60" },
                    new SelectListItem { Text = "90", Value = "90" },
                    new SelectListItem { Text = "120", Value = "120" },
                };
        }

        public int Id { get; set; }
       // [Required(ErrorMessage = "Question Paper name is Required.")]
        public string Name { get; set; }

        [Display(Name = "Duration")]
        public int? Minute { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public List<QuestionPaperMapping> QuestionPaperMappings { get; set; }
        public List<QuestionPaperViewModel> QuestionPaperList { get; set; }
        public QuestionBankSearchViewModel QuestionBankSearchViewModel { get; set; }
        public List<SelectListItem> Minutes { get; set; }
    }
    public class QuestionPaperViewModel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public bool IsSelected { get; set; } // only for question paper
        public string Name { get; set; }

        [AllowHtml]
        public string Decription { get; set; }
        public string QuestionFormat { get; set; }
    }
}