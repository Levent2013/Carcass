using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Carcass.Models
{
    /// <summary>
    /// Display-only user model
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }

        public int BlogPostsCount { get; set; }


    }
}