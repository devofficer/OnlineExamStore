using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class Test
    {
        public Test()
        {
            Sections = new List<Section>();
        }
        public int QuestionPaperId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public int QuestionCount { get; set; }
        public int TotalMarks { get; set; }
        public List<Section> Sections { get; set; }
    }
}