using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Followers
    {
        [Key]
        public long Id { get; set; }
        public int UserProfileId { get; set; }
        public int FollowersUserProfileId { get; set; }
        public int? GroupId { get; set; }
        public DateTime StartDate { get; set; }
    }
}