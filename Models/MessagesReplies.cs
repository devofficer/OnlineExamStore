using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class MessagesReplies
    {
        [Key]
        public long Id { get; set; }
        public Guid ReplyGuId { get; set; }
        public long MessageId { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "Reply is required")]
        public string ReplyText { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}