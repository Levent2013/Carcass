using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Carcass.Resources;

namespace Carcass.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// Timezone offset in minutes
        /// </summary>
        public string TimeZoneId { get; set; }
    }
}