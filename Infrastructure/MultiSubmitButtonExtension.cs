using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Web.Mvc;

namespace OnlineExam.Infrastructure
{
    public static class JsonExtensions
    {
        public static string ToJson(this Object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }
    }
    public static class MultiSubmitButtonExtension
    {
        public static MvcHtmlString MultiSubmitButton(this HtmlHelper helper, string url, string name)
        {
            return MultiSubmitButton(helper, url, name, null, (IDictionary<string, object>)null);
        }
        public static MvcHtmlString MultiSubmitButton(this HtmlHelper helper, string url, string name, string buttonText)
        {
            return MultiSubmitButton(helper, url, name, buttonText, null);
        }
        public static MvcHtmlString MultiSubmitButton(this HtmlHelper helper, string url)
        {
            return MultiSubmitButton(helper, url, null, null, (IDictionary<string, object>)null);
        }
        public static MvcHtmlString MultiSubmitButton(this HtmlHelper helper, string url, string name, string buttonText, object htmlAttributes)
        {
            return helper.MultiSubmitButton(url, name, buttonText, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static MvcHtmlString MultiSubmitButton(this HtmlHelper helper, string url, string name, string buttonText, IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();

            // Add onClick handler
            string onClick = "$(this).parents('form').attr('action', '" + url + "');";
            if (htmlAttributes.ContainsKey("onClick"))
            {
                htmlAttributes["onClick"] = htmlAttributes["onClick"] + ";" + onClick;
            }
            else
            {
                htmlAttributes.Add("onClick", onClick);
            }
            return helper.SubmitButton(name, buttonText, htmlAttributes);
        }
    }
}