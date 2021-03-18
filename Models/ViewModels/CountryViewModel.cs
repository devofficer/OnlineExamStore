using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Models
{
    public class CountryViewModel
    {
        public string SelectedItem { get; set; }
        public List<SelectListItem> ListItems { get; set; }
    }
}