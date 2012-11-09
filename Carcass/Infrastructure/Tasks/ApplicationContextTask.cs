using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;


using Autofac.Integration.Mvc;
using MvcExtensions;
using WebMatrix.WebData;

using Carcass.Common.Data;
using Carcass.Common.MVC;
using Carcass.Models;

namespace Carcass.Infrastructure.Tasks
{
    public class ApplicationContextTask : PerRequestTask
    {
        static ApplicationContextTask()
        {
        }


        public ApplicationContextTask(IQueryBuilder queryBuilder)
        {
            Query = queryBuilder;
        }

        protected IQueryBuilder Query { get; set; }

        public override TaskContinuation Execute()
        {
            var context = new ApplicationContext();
            var user = Query.Find<UserProfile>(WebSecurity.CurrentUserId);
            if (user != null)
            {
                // context.TimeZoneOffset = user.TimeZoneId;
            }
            

            return TaskContinuation.Continue;
        }
    }
}