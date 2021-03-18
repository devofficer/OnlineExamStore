using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OnlineExam.Infrastructure
{
    public class RequiredIfAttribute : ValidationAttribute//, IClientValidatable
    {
        private String PropertyName { get; set; }
        private String ErrMessage { get; set; }
        private Object DesiredValue { get; set; }
        protected RequiredAttribute innerAttribute;

        public bool AllowEmptyStrings
        {
            get
            {
                return innerAttribute.AllowEmptyStrings;
            }
            set
            {
                innerAttribute.AllowEmptyStrings = value;
            }
        }


        public RequiredIfAttribute(String propertyName, Object desiredValue, String errorMessage)
        {
            this.PropertyName = propertyName;
            this.DesiredValue = desiredValue;
            this.ErrMessage = errorMessage;
            innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            // get a reference to the property this validation depends upon
            //var containerType = context.ObjectInstance.GetType();
            //var field = containerType.GetProperty(PropertyName);

            //if (field != null)
            //{
            //    // get the value of the dependent property
            //    var dependentValue = field.GetValue(context.ObjectInstance, null);
            //    // trim spaces of dependent value
            //    if (dependentValue != null && dependentValue is string)
            //    {
            //        dependentValue = (dependentValue as string).Trim();

            //        if (!AllowEmptyStrings && (dependentValue as string).Length == 0)
            //        {
            //            dependentValue = null;
            //        }
            //    }

            //    // compare the value against the target value
            //    if ((dependentValue == null && DesiredValue == null) ||
            //        (dependentValue != null && (DesiredValue == "*" || dependentValue.Equals(DesiredValue))))
            //    {
            //        // match => means we should try validating this field
            //        if (!innerAttribute.IsValid(value))
            //            // validation failed - return an error
            //            return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
            //    }
            //}

            Object instance = context.ObjectInstance;
            Type type = instance.GetType();
            var field = type.GetProperty(PropertyName);

            if (field != null)
            {
                Object proprtyvalue = field.GetValue(context.ObjectInstance, null);
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = "requiredif"
            };
            string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            string targetValue = (DesiredValue ?? "").ToString();
            if (DesiredValue is bool)
            targetValue = targetValue.ToLower();

            rule.ValidationParameters.Add("dependentproperty", depProp);
            rule.ValidationParameters.Add("targetvalue", targetValue);

            yield return rule;
        }
        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext)
        {
            // build the ID of the property
            string depProp = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(PropertyName);
            // unfortunately this will have the name of the current field appended to the beginning,
            // because the TemplateInfo's context has had this fieldname appended to it. Instead, we
            // want to get the context as though it was one level higher (i.e. outside the current property,
            // which is the containing object, and hence the same level as the dependent property.
            var thisField = metadata.PropertyName + "_";
            if (depProp.StartsWith(thisField))
                // strip it off again
                depProp = depProp.Substring(thisField.Length);
            return depProp;
        }
    }
}