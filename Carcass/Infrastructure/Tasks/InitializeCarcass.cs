using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Carcass.Data;
using MvcExtensions;
using log4net;

namespace Carcass.Infrastructure.Tasks
{
    public class InitializeCarcass : BootstrapperTask
    {
        public InitializeCarcass()
        {
            Log = LogManager.GetLogger("InitializeCarcass");
        }


        private ILog Log { get; set; }

        public override TaskContinuation Execute()
        {
            var appAssembly = typeof(Carcass.MvcApplication).Assembly;

            Carcass.Common.MVC.Bootstrap.Init();
            Carcass.Common.MVC.Bootstrap.LocalizeModels(appAssembly, "Carcass.Models");

            return TaskContinuation.Continue;
        }
    }
}