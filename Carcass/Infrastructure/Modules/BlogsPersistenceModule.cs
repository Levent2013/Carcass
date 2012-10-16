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
    public class BlogsPersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
      
            #region Register Type Mappings

          
            #endregion
            
            #region Register Data Access components

            builder.Register<IRepository<BlogPostEntity>>(container => new Repository<BlogPostEntity>(container.Resolve<DatabaseContext>().BlogPosts));
            
            #endregion

            base.Load(builder);

        }
    }
}