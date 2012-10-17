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
        #region Fields

        private string _annotation;

        #endregion

        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Origin { get; set; }

        public string Content { get; set; }

        public string Annotation 
        {
            get 
            { 
                if (_annotation == null)
                    _annotation = MvcHelper.GetHtmlPreview(Content);
                
                return _annotation;
            }
            
            set 
            { 
                _annotation = value; 
            } 
        }

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
                    : Carcass.Resources.AccountResources.UnknownUser; }
        }

        #endregion
    }
}