using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Infrastructure
{
    public class DateValidationAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' must be greater than '{1}'";
        private string _basePropertyName;
        private string _basePropertyDisplayName;

        public DateValidationAttribute(string basePropertyName)
            : base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(_defaultErrorMessage, name, _basePropertyName);
        }
        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            //Get Value of the property  
            try
            {
                var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
                var thisDate = (DateTime)value;

                //Actual comparision  
                if (thisDate <= startDate) //'To' date should not be less than 'From' date
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
                else if (thisDate.ToUniversalTime().Date < DateTime.Now.ToUniversalTime().Date)
                {
                    return new ValidationResult("'Visit To Date' should not be less than 'Current' date.");
                }

                else if (startDate.ToUniversalTime().Date < DateTime.Now.ToUniversalTime().Date)
                {
                    return new ValidationResult("'Visit From Date' should not be less than 'Current' date.");
                }
                else if ((thisDate.Subtract(startDate)).Days > 365)
                {
                    return new ValidationResult("Difference between 'Visit From Date' and 'Visit To Date' should not exceed more than 365 days ");
                }
            }
            catch
            {
                return new ValidationResult("'The provided date is in Invalid date format.");
            }
            //Default return - This means there were no validation error  
            return null;
        }
    }
}