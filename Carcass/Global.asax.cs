using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;
using MvcExtensions;
using WebMatrix.WebData;

using log4net;
using Carcass.Infrastructure.Tasks;

namespace Carcass
{
    /// <summary>
    /// Integrated MvcExtensions: https://github.com/MvcExtensions/Core/wiki/Getting-started-with-MvcExtensions 
    /// </summary>
    public class MvcApplication : MvcExtensions.Autofac.AutofacMvcApplication
    {
        private ILog log = LogManager.GetLogger("Application");

        public MvcApplication()
        {
            Bootstrapper.BootstrapperTasks
                .Include<InitializeDatabase>()
                .Include<RegisterRoutes>()
                .Include<InitializeCarcass>()
                .Include<RegisterAreas>()
                .Include<RegisterControllers>()
                .Include<RegisterModelMetadata>()
                //.Include<RegisterModelBinders>()
                //.Include<RegisterActionInvokers>()
                .Include<Infrastructure.RegisterPerRequestServices>(ConfigureRegisterPerRequestServices);
               
            Error += OnError;
        }

        private void OnError(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            log.Error(exception.Message, exception);
            // Server.ClearError();
        }

        private void ConfigureRegisterPerRequestServices(Infrastructure.RegisterPerRequestServices task)
        {
            task.Adapter = Adapter;
        }

        protected override void OnStart()
        {
            log.Debug("------------ Application_Start ------------ ");

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            Bootstrapper.PerRequestTasks.Include<LocalizationTask>();
#if DEBUG
            Bootstrapper.PerRequestTasks.Include<CheckDatabaseTask>();
#endif
            InitializeMappings.Init();
            
        }

        protected override void OnEnd()
        {
            log.Debug("------------ Application_End ------------ ");
        }
    }
}