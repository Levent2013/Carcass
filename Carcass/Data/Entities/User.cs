using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web;

namespace Carcass.Data.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        
        public string UserName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateRegistered { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName 
        {
            get { return (FirstName + " " + LastName).Trim(); } 
        }


    }
}