using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class UserPayments
    {
        [Key]
        public long Id { get; set; }
        public Guid PaymentGuId { get; set; }
        public int ProfileId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string TransactionMessage { get; set; }
        public string TransactionStatus { get; set; }
        public string Bank { get; set; }
        public string TxnId { get; set; }
        public string ProofFile { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedUserProfileId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Note { get; set; }
    }
}