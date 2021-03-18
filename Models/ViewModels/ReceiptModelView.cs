using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OnlineExam.Utils;

namespace OnlineExam.Models
{
    public class ReceiptModelView
    {
        [Key]
        public int InvoiceId { get; set; }

        public int CompanyId { get; set; }
        public bool IsReceiptAlreadyCreated { get; set; }

        [Required]
        [Display(Name = "Invoice Code")]
        public string InvoiceCode { get; set; }
        [Display(Name = "Invoice Code")]
        public string ReceiptCode { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Address")]
        public string CompanyAddress { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Received Amount")]
        public decimal ReceivedAmount { get; set; }

        [Required]
        [Display(Name = "Payment Mode")]
        public PaymentMode ModeOfPayment { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Cheque No")]
        public string ChequeNo { get; set; }

        [Display(Name = "Cheque Date")]
        public DateTime? ChequeDate { get; set; }

        public string Message { get; set; }
    }
}