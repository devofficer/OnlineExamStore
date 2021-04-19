using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class MessagesViewModel
    {
    }
    public class MessageDetailsViewModel
    {
        public long Id { get; set; }
        public Guid MessageGuId { get; set; }
        public string AssignUserType { get; set; }
        public string ClassType { get; set; }
        public int? AssignUserId { get; set; }
        public string MessageText { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string UserAvater { get; set; }
        public string CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string ReadUserIds { get; set; }
        public bool ReplyAllowed { get; set; }
        public int ReplyCount { get; set; }
    }

    public class MessagesRepliesViewModel
    {
        public long Id { get; set; }
        public Guid ReplyGuId { get; set; }
        public long MessageId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvater { get; set; }
        public string ReplyText { get; set; }
        public string CreatedDate { get; set; }
    }
}