using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class TestViewModel
    {
        public TestViewModel()
        {
            QuizViewModel = new List<QuizViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }

        public string Format { get; set; }
        public List<QuizViewModel> QuizViewModel { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}