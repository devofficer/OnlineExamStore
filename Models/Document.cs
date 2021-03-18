using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public byte[] RowData { get; set; }

        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }

        public Company Company { get; set; }
        public int CompanyId { get; set; }
    }
}