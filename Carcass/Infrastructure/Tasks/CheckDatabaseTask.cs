using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


using Autofac.Integration.Mvc;
using MvcExtensions;
using WebMatrix.WebData;

namespace Carcass.Infrastructure.Tasks
{
    public class CheckDatabaseTask : PerRequestTask
    {
        public CheckDatabaseTask(Data.DatabaseContext context)
        {
        }

        public override TaskContinuation Execute()
        {
            var url = HttpContext.Current.Request.Url;
            if (url.AbsolutePath.Contains("Content/css") ||
                url.AbsolutePath.Contains("Content/js") ||
                url.AbsolutePath.Contains("Content/img") ||
                url.AbsolutePath.Contains("bundles/"))
            {
                return TaskContinuation.Continue;
            }

            var dbContext = DependencyResolver.Current.GetService<Data.DatabaseContext>();

            
            if (!dbContext.Database.Exists())
            {
                dbContext.Database.Initialize(true);
            }
                        
            return TaskContinuation.Continue;
        }
    }
}