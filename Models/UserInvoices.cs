using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class UserInvoices
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int ProfileId { get; set; }
        public string EnrollType { get; set; }
        public int EnrollId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}