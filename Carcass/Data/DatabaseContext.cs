using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;

using Carcass.Data.Entities;

namespace Carcass.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext():
            base("DefaultConnection")
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<BlogPostEntity> BlogPosts { get; set; }
    }
}