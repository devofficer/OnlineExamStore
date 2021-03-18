using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;
using StructureMap.Graph;
using Registry = StructureMap.Configuration.DSL.Registry;

namespace OnlineExam.Infrastructure.Tasks
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.FullName.StartsWith("OnlineExam"));
                scan.AddAllTypesOf<IRunAtInit>();
                scan.AddAllTypesOf<IRunAtStartup>();
                scan.AddAllTypesOf<IRunOnEachRequest>();
                scan.AddAllTypesOf<IRunOnError>();
                scan.AddAllTypesOf<IRunAfterEachRequest>();
            });
        }
    }
}