using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class MessageDetails
    {
        [Key]
        public long Id { get; set; }
        public Guid MessageGuId { get; set; }
        [Required(ErrorMessage = "User type is required")]
        public string AssignUserType { get; set; }
        public string ClassType { get; set; }
        public int? AssignUserId { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string MessageText { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string ReadUserIds { get; set; }
        public bool ReplyAllowed { get; set; }
    }
}