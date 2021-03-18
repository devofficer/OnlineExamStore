using OnlineExam.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace OnlineExam.Models
{
    public class UserBankPayment
    {
        public string Account
        {
            get;
            set;
        }

        public double Amount
        {
            get;
            set;
        }

        public string Bank
        {
            get;
            set;
        }

        public string Beneficiary
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Key]
        public int Id
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public DateTime? PaymentOn
        {
            get;
            set;
        }

        public PaymentStatus PaymentStatus
        {
            get;
            set;
        }

        public string TxnId
        {
            get;
            set;
        }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }

        public UserBankPayment()
        {
            this.IsActive = true;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}