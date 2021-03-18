using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PastDateAttribute : ValidationAttribute, IClientValidatable
    {
         private const string ErrMessage = "'{0}' must be past date.";
        public PastDateAttribute()
            : base(ErrMessage)
        {

        }
        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var parsedDate = (DateTime) value;
                if (parsedDate.ToUniversalTime().Date >= DateTime.Now.ToUniversalTime().Date)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "pastdate",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            yield return rule;
        }
    }
}