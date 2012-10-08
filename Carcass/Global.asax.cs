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
using WebMatrix.WebData;

using log4net;

namespace Carcass
{
    // TODO: refactor with MvcExtensions
    // https://github.com/MvcExtensions/Core/wiki/Getting-started-with-MvcExtensions
    // https://github.com/MvcExtensions/Core/wiki/IoC-Integration%3A-Autofac
    // https://github.com/MvcExtensions/Core/wiki/Route-registration
    public class MvcApplication : System.Web.HttpApplication
    {
        private ILog _log = LogManager.GetLogger("Application");
        private Carcass.Common.MVC.Bootstrap _carcassBootstrap = new Common.MVC.Bootstrap();

        protected void Application_Start()
        {
            _log.Debug("------------ Application_Start ------------ ");
            InitializeDependencyResolver();
            InitializeDatabase();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

             _carcassBootstrap.Init();
        }

        protected void Application_End()
        {
            _log.Debug("------------ Application_End ------------ ");
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            try
            {
                var context = AutofacDependencyResolver.Current.GetService<Data.DatabaseContext>();
                if (!context.Database.Exists())
                {
                    context.Database.Initialize(true);
                }

                if (!WebSecurity.Initialized)
                    Data.DatabaseContextInitializer.InitializeMembership();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to initialize database", ex);
                throw;
            }
        }
               
        private void InitializeDatabase()
        {
            Database.SetInitializer<Data.DatabaseContext>(new Data.DatabaseContextInitializer());
        }

        private void InitializeDependencyResolver()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<Data.DatabaseContext>().InstancePerHttpRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}