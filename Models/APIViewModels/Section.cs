using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class Section
    {
        public Section()
        {
            Questions = new List<Question>();
            Stats = new Stats();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeSpent { get; set; }
        public string Instruction { get; set; }
        public List<Question> Questions { get; set; }
        public Stats Stats { get; set; }
    }
}