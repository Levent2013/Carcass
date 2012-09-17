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
    public class BlogPost
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BlogPostId { get; set; }
        
        public int? UserId { get; set; }

        public string UserName { get; set; }
        
        [DataType(DataType.Text)]
        [StringLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// Post content
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateModified { get; set; }
        
        public virtual User User { get; set; } 
    }
}