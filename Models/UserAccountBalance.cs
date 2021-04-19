using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class UserAccountBalance
    {
        [Key]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}