using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;
using Carcass.Models;

namespace Carcass.Models.Configurations
{
    public class BlogPostConfiguration : ModelMetadataConfiguration<BlogPost>
    {
        public BlogPostConfiguration()
        {
            Configure(model => model.Id).Hide();
            Configure(model => model.Title).Required().MaximumLength(255).Order(1);
            Configure(model => model.Origin).MaximumLength(1024).Order(2).AsUrl();
            Configure(model => model.Content).Required().Order(3).AsHtml();

            Configure(model => model.DateCreated).Hide();
            Configure(model => model.DateModified).Hide();
            Configure(model => model.Preview).Hide();
            Configure(model => model.Author).Hide();
            Configure(model => model.AuthorId).Hide();
            Configure(model => model.AuthorName).Hide();
            Configure(model => model.CommentsCount).Hide();
            Configure(model => model.CommentsEnabled).Hide();
        }
    }
}