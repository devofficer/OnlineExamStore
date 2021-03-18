using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Tax
    {
        public Tax()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int TaxId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAppliedOnBasePrice { get; set; }
        public decimal TaxValue { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}