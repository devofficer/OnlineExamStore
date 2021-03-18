using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class ClassViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Discription { get; set; }
        public bool IsActive { get; set; }
    }
    public class QuestionFormatViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Discription { get; set; }
        public bool IsActive { get; set; }
    }
}