using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

using Carcass.Resources;

namespace Carcass.Models
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "UserName", ResourceType = typeof(AccountResources))]
        public string UserName { get; set; }

        public string OriginalUserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "UserName", ResourceType = typeof(AccountResources))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountResources))]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "UserName", ResourceType = typeof(AccountResources))]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountResources))]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        
        public string ProviderDisplayName { get; set; }
        
        public string ProviderUserId { get; set; }
    }
}
