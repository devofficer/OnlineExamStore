﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Infrastructure
{
    public class FileUploadJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            ContentType = "text/html";
            context.HttpContext.Response.Write("<textarea>");
            base.ExecuteResult(context);
            context.HttpContext.Response.Write("</textarea>");
        }
    }
}