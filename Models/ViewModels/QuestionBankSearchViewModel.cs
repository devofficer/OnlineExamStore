using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Models
{
    public class QuestionBankSearchViewModel
    {
        public QuestionBankSearchViewModel()
        {
            ClasseTypes = new HashSet<SelectListItem>();
            ExamTypes = new HashSet<SelectListItem>();
            Subjects = new HashSet<SelectListItem>();
            QuestionPaperList = new List<QuestionPaperViewModel>();
            QuestionFormats = new HashSet<SelectListItem>();

            PageSizeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "20", Value = "20" },
                    new SelectListItem { Text = "30", Value = "30" },
                    new SelectListItem { Text = "40", Value = "40" },
                    new SelectListItem { Text = "50", Value = "50" }
                };
            MarkList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "1", Value = "1" },
                    new SelectListItem { Text = "2", Value = "2" },
                    new SelectListItem { Text = "3", Value = "3" },
                    new SelectListItem { Text = "4", Value = "4" },
                    new SelectListItem { Text = "5", Value = "5" }
                };

            PageSize = 10;
            PageIndex = 1;
            CurrentFilter = string.Empty;
            IsOnline = true;
        }

        [Display(Name = "Search By Question Id:")]
        public int QuestionId { get; set; }

        //Paging parameters
        // [Required(ErrorMessage = "Question Paper name is Required.")]
        public string Name { get; set; }

        [Display(Name = "Duration")]
        public int? Minute { get; set; }
        public string CurrentFilter { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }


        public int? Mark { get; set; }
        public bool IsOnline { get; set; }
        public string ClassName { get; set; }
        public string ExamName { get; set; }
        public string Subject { get; set; }
        public string QuestionFormat { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<SelectListItem> PageSizeList { get; set; }
        public IEnumerable<SelectListItem> MarkList { get; set; }

        public IEnumerable<SelectListItem> ClasseTypes { get; set; }
        public IEnumerable<SelectListItem> ExamTypes { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }
        public IEnumerable<SelectListItem> QuestionFormats { get; set; }

        public List<QuestionPaperViewModel> QuestionPaperList { get; set; }
    }
}