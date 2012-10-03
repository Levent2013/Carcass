using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Carcass.Common.MVC.DataAnnotations
{
    /// <summary>
    /// Setup file input properties 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FileUploadAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FileUploadAttribute()
        {
            Multiple = false;
        }

        /// <summary>
        /// Flag to enable multiple files selection
        /// </summary>
        public bool Multiple { get; set; }
    }
}
