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

        private string _preview;

        #endregion

        public BlogPost()
        {
            Author = new User();
        }
            
        
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Origin { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public User Author { get; set; }

        public int CommentsCount { get; set; }

        public bool CommentsEnabled { get; set; }

        #region Calculable properties

        public string Preview
        {
            get
            {
                if (_preview == null)
                {
                    lock(this)
                    {
                        if (_preview == null)
                        {
                            _preview = MvcHelper.GetHtmlPreview(Content);
                        }
                    }
                }

                return _preview;
            }
        }

        public string AuthorName
        {
            get { return Author.FullName ?? Carcass.Resources.AccountResources.UnknownUser; }
        }

        public int AuthorId
        {
            get { return Author.Id; }
        }

        #endregion
    }
}