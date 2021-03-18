using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class Question
    {
        public Question()
        {
            Options = new List<Option>();
            OptionType = OptionType.Single;
            ChildQuestions = new List<Question>();
        }
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string Solution { get; set; }
        public int Mark { get; set; }
        public int Duration { get; set; }
        public int TimeSpent { get; set; }
        public bool IsOnline { get; set; }
        public int ParentId { get; set; }
        public string Exam { get; set; }
        public string Subject { get; set; }
        public string Format { get; set; }
        public string UserOption { get; set; }
        public string AnswerOption { get; set; }
        public string ImagePath { get; set; }
        //Ignore them, these are NOT database properties
        public bool IsBooked { get; set; }
        public bool IsAttempted { get; set; }
        public bool IsSkipped { get; set; }
        public OptionType OptionType { get; set; }
        public List<Option> Options { get; set; }

        //Added on 09th March 2017
        public List<Question> ChildQuestions { get; set; }
    }
}