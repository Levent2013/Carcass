using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

using Carcass.Common.MVC.DataAnnotations;
using Carcass.Common.MVC.DataTypes;

namespace Carcass.Models
{
    public class ComplexModel
    {
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Currency { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [DataType(DataType.Duration)]
        public decimal Duration { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

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

        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [DataType(DataType.Upload)]
        [FileUpload(Multiple = true)]
        public PostedFile[] MultipleUpload { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }
    }
}
