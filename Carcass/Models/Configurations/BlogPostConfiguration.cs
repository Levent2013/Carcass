using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;
using Carcass.Common.Resources;
using Carcass.Models;


namespace Carcass.Models.Configurations
{
    public class BlogPostConfiguration : ModelMetadataConfiguration<BlogPost>
    {
        public BlogPostConfiguration()
        {
            Configure(model => model.Id).Hide();
            Configure(model => model.Title).Required().MaximumLength(255).Order(1);
            Configure(model => model.Origin).MaximumLength(1024, () => ValidationResources.MaxLength).Order(2).AsUrl();
            Configure(model => model.Content).Required().Order(3).AllowHtml().AsHtml();
            Configure(model => model.Annotation).MaximumLength(512, () => ValidationResources.MaxLength).Order(4).AllowHtml().AsHtml();

            Configure(model => model.DateCreated).Hide();
            Configure(model => model.DateModified).Hide();
            Configure(model => model.Author).Hide();
            Configure(model => model.AuthorId).Hide();
            Configure(model => model.AuthorName).Hide();
            Configure(model => model.CommentsCount).Hide();
            Configure(model => model.CommentsEnabled).Hide();
        }
    }
}