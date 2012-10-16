using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Carcass.Common.Data;
using Carcass.Common.Data.Extensions;

using Carcass.Models;
using Carcass.Data;
using Carcass.Data.Entities;



namespace Carcass.Infrastructure.Modules
{
    public class CommonPersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // InstancePerHttpRequest() does not work in Autofack 2.6.3.862
            // builder.RegisterType<Data.DatabaseContext>().InstancePerHttpRequest();

            builder.RegisterType<QueryBuilder>().As<IQueryBuilder>();

            base.Load(builder);
        }
    }
}