using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace OnlineExam.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryText { get; set; }
        public string ContactCountryCode { get; set; }
    }
    public class State
    {
        [Key]
        public int StateId { get; set; }
        public string StateCode { get; set; }
        public string StateText { get; set; }


        //[ForeignKey("CountryCode")]
        //public Country Country { get; set; }
        public string CountryCode { get; set; }
    }
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string CityCode { get; set; }
        public string CityText { get; set; }


        //[ForeignKey("StateCode")]
        //public State State { get; set; }
        public string StateCode { get; set; }
    }
    public class PostCode
    {
        [Key]
        public int PostCodeId { get; set; }
        public string Postcode { get; set; }


        [ForeignKey("StateId")]
        public State State { get; set; }
        public int StateId { get; set; }
       
    }
}