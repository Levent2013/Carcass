﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;


using Autofac.Integration.Mvc;
using MvcExtensions;
using WebMatrix.WebData;

namespace Carcass.Infrastructure.Tasks
{
    public class LocalizationTask : PerRequestTask
    {
        public LocalizationTask()
        {
        }

        public override TaskContinuation Execute()
        {
            var globalization = WebConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;
            if (globalization != null && globalization.EnableClientBasedCulture)
            {
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
           
            return TaskContinuation.Continue;
        }
    }
}