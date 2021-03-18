using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExam.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [ForeignKey("GroupId")]
        public Message GroupMessage { get; set; }
        public int? GroupId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public int CompanyId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
        public string RepliedBy { get; set; }

    }
}