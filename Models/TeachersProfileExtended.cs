using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace OnlineExam.Models
{
    public class TeachersProfileExtended
    {
        [Key]
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Expertise { get; set; }
        public string Qualifications { get; set; }
        public string Offering { get; set; }
        public string Lessons { get; set; }

    }
}