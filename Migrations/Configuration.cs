using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using OnlineExam.Utils;

namespace OnlineExam.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<OnlineExam.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "OnlineExam.Models.ApplicationDbContext";
        }

        protected override void Seed(OnlineExam.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}
