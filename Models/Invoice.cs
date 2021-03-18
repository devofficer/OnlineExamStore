using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Ocsp;

namespace OnlineExam.Models
{
    public class Invoice
    {
        public Invoice()
        {
            InvoiceItems = new List<InvoiceItem>();
            //Receipt = new Receipt();
        }
        [Key]
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string AddressTo { get; set; }
        public string Template { get; set; }
        public string RefNumber { get; set; }
 
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        //[ForeignKey("ReceiptId")]
        public Receipt Receipt { get; set; }
        //public int ReceiptId { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}