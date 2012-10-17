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
            
            #region Register Data Access components

            builder.Register<IRepository<BlogPostEntity>>(container => new Repository<BlogPostEntity>(container.Resolve<DatabaseContext>().BlogPosts));

            builder.Register<IRepository<BlogPost>>(container =>
            {
                var context = container.Resolve<DatabaseContext>();
                return new Repository<BlogPost>(
                    context.BlogPosts.Select(p => new BlogPost
                    {
                        Id = p.BlogPostEntityId,
                        Title = p.Title,
                        Origin = p.Origin,
                        Content = p.Content,
                        DateCreated = p.DateCreated,
                        DateModified = p.DateModified,
                        AuthorId = p.UserEntityId ?? 0,
                        Author = new User
                            { 
                                Id = p.UserEntityId ?? 0,
                                Email = p.UserEntity.Email,
                                UserName = p.UserEntity.UserName,
                                FirstName = p.UserEntity.FirstName,
                                LastName = p.UserEntity.LastName,
                                BlogPostsCount = p.UserEntity.BlogPosts.Count(),
                            },
                        
                        CommentsEnabled = p.CommentsEnabled,
                        // CommentsCount = p.Comments.Count()
                    }));
            });

            
            builder.Register<IFinder<BlogPost>>(
              container =>
                  new EntityFinder<BlogPost, DatabaseContext>(
                      container.Resolve<DatabaseContext>(),
                      (context, id) => context.BlogPosts.Find(id).MapTo<BlogPost>())
            );

            builder.Register<ILookuper<BlogPost>>(container =>
            {
                var context = container.Resolve<DatabaseContext>();
                return new EntityLookuper<BlogPost, BlogPostEntity>(
                    context /* DB context */,
                    context.BlogPosts /* target table */,
                    p => p.Id /* primary key load function */,
                    p => 
                    {
                        if(p.BlogPostEntityId == 0)
                            p.DateCreated = ServerTime.Now;
                        p.DateModified = ServerTime.Now;
                    });
            });


            #endregion

            base.Load(builder);

        }
    }
}