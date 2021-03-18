using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using ExpressiveAnnotations.Attributes;
using ExpressiveAnnotations.MvcUnobtrusiveValidatorProvider.Validators;
using OnlineExam.Models;
using StructureMap;
using OnlineExam.Infrastructure;
using OnlineExam.Infrastructure.Tasks;
using System.Web.Http;

namespace OnlineExam
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            // TO ENABLE SESSION BASED API
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        protected void Application_Start()
        {
           // Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());

            AreaRegistration.RegisterAllAreas();
            // Manually installed WebAPI 2.2 after making an MVC project.
            GlobalConfiguration.Configure(WebApiConfig.Register); // NEW way
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           // DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(RequiredIfValidator));

            //var cInfo = new CultureInfo("en-AU")
            //{
            //    DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", DateSeparator = "/" }
            //};
            //Thread.CurrentThread.CurrentCulture = cInfo;
            //Thread.CurrentThread.CurrentUICulture = cInfo;

            ObjectFactory.Configure(cfg =>
            {
                cfg.AddRegistry(new TaskRegistry());
            });

            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                foreach (var task in container.GetAllInstances<IRunAtInit>())
                {
                    task.Execute();
                }
                foreach (var task in container.GetAllInstances<IRunAtStartup>())
                {
                    task.Execute();
                }
            }
        }

        protected void Application_BeginRequest()
        {
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                foreach (var task in container.GetAllInstances<IRunOnEachRequest>())
                {
                    task.Execute();
                }
            }
        }
        protected void Application_Error()
        {
            try
            {
                using (var container = ObjectFactory.Container.GetNestedContainer())
                {
                    foreach (var task in container.GetAllInstances<IRunOnError>())
                    {
                        task.Execute();
                    }
                }
            }
            finally
            {

            }
        }
        protected void Application_EndRequest()
        {
            try
            {
                using (var container = ObjectFactory.Container.GetNestedContainer())
                {
                    foreach (var task in container.GetAllInstances<IRunOnEachRequest>())
                    {
                        task.Execute();
                    }
                }
            }
            finally
            {

            }
        }
    }
}
