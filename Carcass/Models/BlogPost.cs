using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcExtensions;
using Carcass.Common.MVC;

namespace Carcass.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Origin { get; set; }

        public string Content { get; set; }

        public string Annotation { get; set; }
        
        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int AuthorId { get; set; }
        
        public User Author { get; set; }

        public int CommentsCount { get; set; }

        public bool CommentsEnabled { get; set; }

        #region Calculable properties

        public string AuthorName
        {
            get { return Author != null && !String.IsNullOrEmpty(Author.FullName) 
                ? Author.FullName
                : Author != null && !String.IsNullOrEmpty(Author.UserName)
                    ? Author.UserName
                    : Carcass.Common.Resources.AccountResources.UnknownUser; }
        }

        #endregion
    }
}