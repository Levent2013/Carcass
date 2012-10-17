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
    public class UsersPersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region Register Data Access components

            builder.Register<IRepository<UserEntity>>(container => new Repository<UserEntity>(container.Resolve<DatabaseContext>().Users));
            
            builder.Register<IRepository<User>>(container => 
                {
                    var context = container.Resolve<DatabaseContext>();
                    return new Repository<User>(
                        context.Users.Select(p => new User
                        {
                            Id = p.UserEntityId,
                            UserName = p.UserName,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Email = p.Email,
                            BlogPostsCount = p.BlogPosts.Count(),
                        }));
                });

            builder.Register<ILookuper<UserProfile>>(container =>
            {
                var context = container.Resolve<DatabaseContext>();
                return new EntityLookuper<UserProfile, UserEntity>(
                    context /* DB context */,
                    context.Users /* target table */,
                    p => p.Id /* primary key load function */);
            });

            builder.Register<IFinder<UserEntity>>(
                container => new EntityFinder<UserEntity, DatabaseContext>(container.Resolve<DatabaseContext>(),
                    (context, id) => context.Users.Find(id)));

            builder.Register<IFinder<User>>(
                container => new EntityFinder<User, DatabaseContext>(container.Resolve<DatabaseContext>(),
                    (context, id) => context.Users.Find(id).MapTo<User>()));
          
            #endregion

            base.Load(builder);

        }
    }
}