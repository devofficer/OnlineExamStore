using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class LessonItems
    {
        [Key]
        public long Id { get; set; }
        public int LessonId { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }
        public string FolderName { get; set; }
        public string FilePath { get; set; }
        public string FileNames { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}