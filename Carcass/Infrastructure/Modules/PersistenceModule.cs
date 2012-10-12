using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;
using Carcass.Common.Data;
using Carcass.Data;
using Carcass.Data.Entities;


namespace Carcass.Infrastructure.Modules
{
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // InstancePerHttpRequest() does not work in Autofack 2.6.3.862
            // builder.RegisterType<Data.DatabaseContext>().InstancePerHttpRequest();
           
            builder.RegisterType<QueryBuilder>().As<IQueryBuilder>();
            builder.Register<IRepository<UserEntity>>(p => new Repository<UserEntity>(p.Resolve<DatabaseContext>().Users));
            builder.Register<IRepository<BlogPostEntity>>(p => new Repository<BlogPostEntity>(p.Resolve<DatabaseContext>().BlogPosts));

            base.Load(builder);

        }
    }
}