using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Models
{
    public class QuestionFilterViewModel
    {
        public QuestionFilterViewModel()
        {
            PageIndex = 1;
            PageSize = 10;
            IsOnline = true;
            Classes = new List<SelectListItem>();
            Categories = new List<SelectListItem>();
            Subjects = new List<SelectListItem>();
            Topics = new List<SelectListItem>();
            Formats = new List<SelectListItem>();
            PageSizeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "20", Value = "20" },
                    new SelectListItem { Text = "30", Value = "30" },
                    new SelectListItem { Text = "40", Value = "40" },
                    new SelectListItem { Text = "50", Value = "50" }
                };
            Marks = new List<SelectListItem>
                {
                    new SelectListItem { Text = "1", Value = "1" },
                    new SelectListItem { Text = "2", Value = "2" },
                    new SelectListItem { Text = "3", Value = "3" },
                    new SelectListItem { Text = "4", Value = "4" },
                    new SelectListItem { Text = "5", Value = "5" }
                };
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
            NoOfQuestions = new List<SelectListItem>
                {
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "20", Value = "20" },
                    new SelectListItem { Text = "30", Value = "30" },
                    new SelectListItem { Text = "40", Value = "40" },
                    new SelectListItem { Text = "50", Value = "50" }
                };
            type = "";
            // QuestionPapars = new List<QuestionPapar>();
            //Questions = new List<Question>();
        }

        [Display(Name = "Trial?")]
        public bool IsTrial { get; set; }


        [Display(Name = "CBT Type")]
        public bool CBTType { get; set; }

        public bool IsAttempted { get; set; }

        [Display(Name = "Class Name")]
        public string SelectedClass { get; set; }

        [Display(Name = "Category")]
        public string SelectedCategory { get; set; }

        [Display(Name = "Subject")]
        public string SelectedSubject { get; set; }

        [Display(Name = "Topic")]
        public string[] SelectedTopic { get; set; }

        [Display(Name = "No Of Questions")]
        public int? SelectedNoOfQuestions { get; set; }

        public string SelectedFormat { get; set; }
        public int? SelectedMark { get; set; }
        public int? SelectedMinute { get; set; }
        public IEnumerable<SelectListItem> Classes { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }
        public IEnumerable<SelectListItem> Topics { get; set; }
        public IEnumerable<SelectListItem> Formats { get; set; }
        public List<SelectListItem> Marks { get; set; }
        public List<SelectListItem> PageSizeList { get; set; }
        public List<SelectListItem> Minutes { get; set; }
        public IEnumerable<SelectListItem> NoOfQuestions { get; set; }

        public PagedList.IPagedList<QuestionPaperListViewModel> QuestionPapars { get; set; }
        public IList<QuestionListViewModel> Questions { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        [Display(Name = "Online?")]
        public bool IsOnline { get; set; }
        public string Name { get; set; }
        [Display(Name = "Duration <br /> (Mins)")]
        public string Duration { get; set; }
        public string type { get; set; }
    }

    public class QuestionListViewModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int Index { get; set; }
        public bool IsSelected { get; set; }
        public int DurationInSecond { get; set; }

        [AllowHtml]
        public string Decription { get; set; }
        public string QuestionFormat { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string AnswerOption { get; set; }
        public string Solution { get; set; }
        public int Mark { get; set; }
        public string Subject { get; set; }
        public string ImagePath { get; set; }
        public string CreatedBy { get; set; }
    }

    public class QuestionPaperDefaultViewModel
    {
        public QuestionPaperDefaultViewModel()
        {
            QuestionPaperSearchViewModel = new QuestionPaperSearchViewModel();
            QuestionPaperList = new List<QuestionPaperListViewModel>();
        }
        public List<QuestionPaperListViewModel> QuestionPaperList { get; set; }
        public QuestionPaperSearchViewModel QuestionPaperSearchViewModel { get; set; }
    }
    public class QuestionPaperSearchViewModel
    {
        public QuestionPaperSearchViewModel()
        {
            PageSizeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "20", Value = "20" },
                    new SelectListItem { Text = "30", Value = "30" },
                    new SelectListItem { Text = "40", Value = "40" },
                    new SelectListItem { Text = "50", Value = "50" }
                };
            PageSize = 10;
            PageIndex = 1;
            CurrentFilter = string.Empty;

            ClasseTypes = new HashSet<SelectListItem>();
            ExamTypes = new HashSet<SelectListItem>();
            Subjects = new HashSet<SelectListItem>();
        }

        public IEnumerable<SelectListItem> ClasseTypes { get; set; }
        public IEnumerable<SelectListItem> ExamTypes { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }

        //[Required]
        [Display(Name = "Class Name", Prompt = "Select Class")]
        public string ClassName { get; set; }

        //[Required]
        public string ExamName { get; set; }

        //[Required]
        public string Subject { get; set; }

        public string CurrentFilter { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public IEnumerable<SelectListItem> PageSizeList { get; set; }
    }

    public class QuestionPaperListViewModel
    {
        public int Index { get; set; }
        public int Id { get; set; }

        public int Status { get; set; } // This prop is used to check, whether question papar is attempted by logged-in user or not
        [Display(Name = "Time Taken In Minutes")]
        public int TimeTakenInMinutes { get; set; }

        [Display(Name = "Total Obtained Marks")]
        public int TotalObtainedMarks { get; set; }

        public int TotalAttemptedQuestions { get; set; }
        public string Name { get; set; }

        // public int Hour { get; set; }
        [Display(Name = "Duration")]
        public int? Minute { get; set; }

        public bool IsTrial { get; set; }
        public bool IsOnline { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalMarks { get; set; }

        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        [Display(Name = "Exam Name")]
        public string ExamName { get; set; }
        public string Subject { get; set; }


        [Display(Name = "CBT Type")]
        public string CBTType { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }


    }
}