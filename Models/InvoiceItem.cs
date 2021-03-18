using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int? ProductId { get; set; }

        public int? SerialNo { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}