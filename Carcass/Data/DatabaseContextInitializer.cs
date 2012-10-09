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
        private static object _initMembershipLock = new object();

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
            lock (_initMembershipLock)
            {
                // Reset WebSecurity internal state - hack, but there is no other way
                ResetWebSecurityInitialization();

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

        /// <summary>
        /// Reset WebSecurity internal state if database 
        /// was updated but WebSecurity.Initialized still true.
        /// <remarks>
        /// Tested with WebMatrix 2.0.0.0
        /// </remarks>
        /// </summary>
        private static void ResetWebSecurityInitialization()
        {
            var initializedProperty = typeof(WebSecurity).GetProperty("Initialized",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
            if (initializedProperty !=null)
                initializedProperty.SetValue(null, false);

            var membership = Membership.Provider as SimpleMembershipProvider;
            if (membership != null)
            {
                initializedProperty = membership.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                if (initializedProperty != null)
                    initializedProperty.SetValue(membership, false);
            }

            var simpleRoles = Roles.Provider as SimpleRoleProvider;
            if (simpleRoles != null)
            {
                initializedProperty = simpleRoles.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                if (initializedProperty != null)
                    initializedProperty.SetValue(simpleRoles, false);
            }
        }

    }
}