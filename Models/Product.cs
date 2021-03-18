using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Product
    {

        public Product()
        {
            InvoiceItems = new HashSet<InvoiceItem>();
        }

        [Key]
        public int ProductId { get; set; }

        [ForeignKey("TaxId")]
        public Tax Tax { get; set; }
        public int? TaxId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int Validity { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public bool IsTaxApplicable { get; set; }
        public string Narration { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}