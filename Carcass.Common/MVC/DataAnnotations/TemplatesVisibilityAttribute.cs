using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Carcass.Common.MVC.DataAnnotations
{
    /// <summary>
    /// Got from http://stackoverflow.com/a/5583825/1742522
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TemplateVisibilityAttribute : Attribute, IMetadataAware
    {
        public bool ShowForDisplay { get; set; }

        public bool ShowForEdit { get; set; }

        public TemplateVisibilityAttribute()
        {
            ShowForDisplay = true;
            ShowForEdit = true;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            metadata.ShowForDisplay = ShowForDisplay;
            metadata.ShowForEdit = ShowForEdit;
        }

    }
}
