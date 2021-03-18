using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class BankPaymentViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// Txn Id should be BANK Voucher code which will display on UI as DEPOSITOR & NARRATION
        /// </summary>
        public string TxnId { get; set; }
        public double Amount { get; set; }
        public string Beneficiary { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public OnlineExam.Utils.PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? PaymentOn { get; set; }
    }
}