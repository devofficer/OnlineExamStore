using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Lookup
    {
        public int LookupId { get; set; }
        public string ModuleCode { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Parent { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
    public class ANZSCOCodes
    {
        public int Id { get; set; }
        public string ANZSCOCode { get; set; }
        public string Occupation { get; set; }
    }
}