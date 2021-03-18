using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
    }
}