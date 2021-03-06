﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Carcass.Common.Data.Extensions;

using Carcass.Models;
using Carcass.Data.Entities;

using AutoMapper;
using MvcExtensions;

namespace Carcass.Infrastructure.Tasks
{
    /// <summary>
    /// Initialize common Automapper mappings here
    /// </summary>
    public class InitializeMappings : BootstrapperTask
    {
        public override TaskContinuation Execute()
        {
            Init();

            return TaskContinuation.Continue;
        }

        public static void Init()
        {
            Mapper.CreateMap<int, string>().ConvertUsing(p => p.ToString(CultureInfo.InvariantCulture));

            #region Users Mappings

            Mapper.CreateMap<UserEntity, UserProfile>()
                .ForMember(u => u.Id, opt => opt.MapFrom(p => p.UserEntityId))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<UserProfile, UserEntity>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<UserEntity, User>()
                .ForMember(u => u.Id, opt => opt.MapFrom(p => p.UserEntityId))
                .ForMember(u => u.UserName, opt => opt.MapFrom(p => p.UserName))
                .ForMember(u => u.FirstName, opt => opt.MapFrom(p => p.FirstName))
                .ForMember(u => u.LastName, opt => opt.MapFrom(p => p.LastName))
                .ForMember(u => u.Email, opt => opt.MapFrom(p => p.Email))
                .ForMember(u => u.BlogPostsCount, opt => opt.MapFrom(p => p.BlogPosts.Count()));
            
            #endregion


            #region Blogs Example Mappings

            Mapper.CreateMap<BlogPost, BlogPostEntity>()
              .ForMember(u => u.BlogPostEntityId, opt => opt.MapFrom(p => p.Id))
              .ForMember(u => u.UserEntityId, opt => opt.MapFrom(p => p.AuthorId))
              .ForMember(u => u.UserEntity, opt => opt.Ignore())
              .ForMember(u => u.DateCreated, opt => opt.Ignore())
              .ForMember(u => u.DateModified, opt => opt.Ignore());
              

            Mapper.CreateMap<BlogPostEntity, BlogPost>()
                .ForMember(u => u.Id, opt => opt.MapFrom(p => p.BlogPostEntityId))
                .ForMember(u => u.AuthorId, opt => opt.MapFrom(p => p.UserEntityId ?? 0))
                .ForMember(u => u.Author, opt => opt.MapFrom(p => p.UserEntity.MapTo<User>()))
                .IgnoreAllNonExisting();

            #endregion


            Mapper.AssertConfigurationIsValid();
        }
    }
}