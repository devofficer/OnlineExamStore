using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class UploadExcelViewModel
    {
        public UploadExcelViewModel()
        {
            InCompleteQuestions = new List<ExcelFormatViewModel>();
        }
        [DisplayName("Select File to Upload")]
        [Required(ErrorMessage = "Please Select File to Upload")]
        public HttpPostedFileBase File { get; set; }

        
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        public string ErrorMsg { get; set; }
        public string SuccessMsg { get; set; }

        [Display(Name = "Topic Format")]
        public bool TopicFormat { get; set; }

        [Display(Name = "Format 1")]
        public bool Format1 { get; set; }
        [Display(Name = "Format 2")]
        public bool Format2 { get; set; }
        [Display(Name = "Format 3")]
        public bool Format3 { get; set; }
        public List<ExcelFormatViewModel> InCompleteQuestions { get; set; }
    }
}