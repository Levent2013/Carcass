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
                        Author = new User
                            { 
                                Id = p.UserEntity == null ? 0 : p.UserEntity.UserEntityId,
                                Email = p.UserEntity == null ? null : p.UserEntity.Email,
                                FirstName = p.UserEntity == null ? null : p.UserEntity.FirstName,
                                LastName = p.UserEntity == null ? null : p.UserEntity.LastName,
                                BlogPostsCount = p.UserEntity == null ? 0 : p.UserEntity.BlogPosts.Count(),
                            },
                        
                        CommentsEnabled = p.CommentsEnabled,
                        // CommentsCount = p.Comments.Count()
                    }));
            });

            #endregion

            base.Load(builder);

        }
    }
}