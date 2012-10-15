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
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // InstancePerHttpRequest() does not work in Autofack 2.6.3.862
            // builder.RegisterType<Data.DatabaseContext>().InstancePerHttpRequest();

            #region Register Type Mappings

            Mapper.CreateMap<UserEntity, UserProfile>()
                .ForMember(u => u.Id, opt => opt.MapFrom(p => p.UserEntityId))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<UserProfile, UserEntity>()
                .ForMember(u => u.Email, opt => opt.MapFrom(p => p.Email))
                .ForMember(u => u.FirstName, opt => opt.MapFrom(p => p.FirstName))
                .ForMember(u => u.LastName, opt => opt.MapFrom(p => p.LastName))
                .IgnoreAllNonExisting();

            #endregion
            
            #region Register Data Access components

            builder.RegisterType<QueryBuilder>().As<IQueryBuilder>();
            builder.Register<IRepository<UserEntity>>(container => new Repository<UserEntity>(container.Resolve<DatabaseContext>().Users));
            builder.Register<IRepository<BlogPostEntity>>(container => new Repository<BlogPostEntity>(container.Resolve<DatabaseContext>().BlogPosts));
            
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
                            BlogPostsCount = p.BlogPosts.Count()
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
            builder.Register<IFinder<BlogPostEntity>>(
                container => new EntityFinder<BlogPostEntity, DatabaseContext>(container.Resolve<DatabaseContext>(),
                    (context, id) => context.BlogPosts.Find(id)));

            #endregion

            base.Load(builder);

        }
    }
}