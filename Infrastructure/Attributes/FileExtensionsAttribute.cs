using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Infrastructure
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileExtensionsAttribute : ValidationAttribute
    {
        private List<string> ValidExtensions { get; set; }

        public FileExtensionsAttribute(string fileExtensions)
        {
            ValidExtensions = fileExtensions.Split('|').ToList();
        }

        public override bool IsValid(object value)
        {
            var file = value as IEnumerable<HttpPostedFileBase>;
            if (file != null)
            {
                foreach (var httpPostedFileBase in file)
                {
                    var fileName = httpPostedFileBase.FileName;
                    var isValidExtension = ValidExtensions.Any(y => fileName.EndsWith(y));
                    return isValidExtension;   
                }
            }
            return true;
        }
    }
}