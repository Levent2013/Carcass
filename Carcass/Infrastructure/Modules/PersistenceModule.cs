using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

namespace Carcass.Infrastructure.Modules
{
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Does not work in Autofack 2.6.3.862
            // builder.RegisterType<Data.DatabaseContext>().InstancePerHttpRequest();
            base.Load(builder);
        }
    }
}