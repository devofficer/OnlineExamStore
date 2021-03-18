using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class Stats
    {
        public int SectionId { get; set; }
        public int Correct { get; set; }
        public int InCorrect { get; set; }
        public int UnAttempted { get; set; }
        public int Bookmarked { get; set; }
    }
}