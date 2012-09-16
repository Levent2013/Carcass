using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Carcass.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}