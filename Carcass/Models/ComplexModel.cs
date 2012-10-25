using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

using Carcass.Common.Resources;
using Carcass.Common.MVC.DataAnnotations;
using Carcass.Common.MVC.DataTypes;

namespace Carcass.Models
{
    public class ComplexModel
    {
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }

        public uint? UnsignedInt { get; set; }

        public int? SignedInt { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Requred")]
        public bool Boolean { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName="Requred")]
        [DataType(DataType.Currency, ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Number")]
        public decimal Currency { get; set; }

        [DataType(DataType.Date, ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Date")]
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Requred")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Time")]
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Requred")]
        public DateTime Time { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DateTime { get; set; }

        [DataType(DataType.Duration)]
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "Requred")]
        public decimal Duration { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        public string Html { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string MultilineText { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [DataType(DataType.Text)]
        public string Text { get; set; }

        [DataType(DataType.Upload)]
        [FileUpload(Multiple = true)]
        public PostedFile[] MultipleUpload { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }
    }
}
