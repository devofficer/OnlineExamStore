using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Receipt
    {
        [Key]
        public int ReceiptId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }

        public string ReceiptCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public string ModeOfPayment { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}