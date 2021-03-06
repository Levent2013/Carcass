﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web;

namespace Carcass.Data.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserEntityId { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateRegistered { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }

        /// <summary>
        /// Timezone Id
        /// </summary>
        [StringLength(255)]
        public string TimeZoneId { get; set; }

        [NotMapped]
        public string FullName 
        {
            get { return (FirstName + " " + LastName).Trim(); } 
        }

        public virtual ICollection<BlogPostEntity> BlogPosts { get; set; }
    }
}