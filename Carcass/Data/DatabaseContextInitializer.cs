using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
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
            // Little hack to reset WebSecurity internal state if database was updated but
            // WebSecurity.Initialized still true
            var initializedProperty = typeof(WebSecurity).GetProperty("Initialized",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
            initializedProperty.SetValue(null, false);

            var membership = Membership.Provider as SimpleMembershipProvider;
            if (membership != null)
            {
                initializedProperty = membership.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                initializedProperty.SetValue(membership, false);
            }

            var simpleRoles = Roles.Provider as SimpleRoleProvider;
            if (simpleRoles != null)
            {
                initializedProperty = simpleRoles.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                initializedProperty.SetValue(simpleRoles, false);
            }

            WebSecurity.InitializeDatabaseConnection(
                "DefaultConnection",
                "Users",
                "UserId",
                "UserName",
                autoCreateTables: true);
            
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