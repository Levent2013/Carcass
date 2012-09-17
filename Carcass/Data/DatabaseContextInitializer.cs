using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WebMatrix.WebData;

namespace Carcass.Data
{
    public class DatabaseContextInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        public DatabaseContextInitializer()
        {
        }

        /// <summary>
        /// Fill the database with inital data (only at development stage)
        /// </summary>
        /// <param name="context">Current DB context</param>
        protected override void Seed(DatabaseContext context)
        {
            InitializeMembership(); 
        }

        public static void InitializeMembership()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(
                    "DefaultConnection",
                    "Users",
                    "UserId",
                    "UserName",
                    autoCreateTables: true);
            }

            // setup default admin
            if (!WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "password", new
                {
                    FirstName = "System",
                    LastName = "Admin",
                    DateRegistered = DateTime.UtcNow
                });
            }
        }

    }
}