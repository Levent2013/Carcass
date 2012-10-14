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
            InitializeViewEngines();
            Carcass.Common.MVC.Bootstrap.Init();

            return TaskContinuation.Continue;
        }

        private void InitializeViewEngines()
        {
            var viewEngines = ViewEngines.Engines;
            var webFormsEngine = viewEngines.OfType<WebFormViewEngine>().FirstOrDefault();
            if (webFormsEngine != null)
            {
                Log.Debug("WebForms ViewEngine disabled");
                viewEngines.Remove(webFormsEngine);
            }
        }
    }
}