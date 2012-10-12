using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web;


namespace Carcass.Data.Entities
{
    [Table("BlogPosts")]
    public class BlogPostEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BlogPostEntityId { get; set; }
        
        public int? UserEntityId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// Post content
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public virtual UserEntity UserEntity { get; set; } 
    }
}