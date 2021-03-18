using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class GenericCompareAttribute : ValidationAttribute, IClientValidatable
    {
        private Enums.CompareOperator operatorName = Enums.CompareOperator.GreaterThanOrEqual;

        public string CompareToPropertyName { get; set; }
        public Enums.CompareOperator OperatorName { get { return operatorName; } set { operatorName = value; } }

        public GenericCompareAttribute() : base() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string operstring = (OperatorName == Enums.CompareOperator.GreaterThan ? "greater than " :
                (OperatorName == Enums.CompareOperator.GreaterThanOrEqual ? "greater than or equal to " :
                (OperatorName == Enums.CompareOperator.LessThan ? "less than " :
                (OperatorName == Enums.CompareOperator.LessThanOrEqual ? "less than or equal to " : ""))));
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);

            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);

            var valThis = (IComparable)value;

            if (valThis != null)
            {
                if ((operatorName == Enums.CompareOperator.GreaterThan && valThis.CompareTo(valOther) <= 0) ||
                    (operatorName == Enums.CompareOperator.GreaterThanOrEqual && valThis.CompareTo(valOther) < 0) ||
                    (operatorName == Enums.CompareOperator.LessThan && valThis.CompareTo(valOther) >= 0) ||
                    (operatorName == Enums.CompareOperator.LessThanOrEqual && valThis.CompareTo(valOther) > 0))
                    return new ValidationResult(base.ErrorMessage);
            }
            return null;
        }
        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            var compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "genericcompare";
            compareRule.ValidationParameters.Add("comparetopropertyname", CompareToPropertyName);
            compareRule.ValidationParameters.Add("operatorname", OperatorName.ToString());
            yield return compareRule;
        }

        #endregion
    }
}