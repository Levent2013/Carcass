﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private ILog _log = LogManager.GetLogger("Application");
        private Carcass.Common.MVC.Bootstrap _carcassBootstrap;

        protected void Application_Start()
        {
            InitializeDependencyResolver();
            InitializeDatabase();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            _carcassBootstrap = new Common.MVC.Bootstrap();
        }

        protected void Application_End()
        {
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
            
            try
            {
                using (var context = new Data.DatabaseContext())
                {
                    context.Database.Initialize(true);
                }
            }
            catch (Exception ex)
            {
                _log.Error("The application database could not be initialized.", ex);
            }
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