using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace OnlineExam
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // TODO: Add any additional configuration code.
            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{action}"
                );
            config.Routes.MapHttpRoute(
                name: "DefaultApiWithId",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //            config.Routes.MapHttpRoute(
            //    name: "GetQuestionPaperData",
            //    routeTemplate: "api/{controller}/{pageNumber}/{pageSize}"
            //);

            // WebAPI when dealing with JSON & JavaScript!
            // Setup json serialization to serialize classes to camel (std. Json format)
            //var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //formatter.SerializerSettings.ContractResolver =new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }
    }
}