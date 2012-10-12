using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Data.Entity;

using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

using WebMatrix.WebData;
using Carcass.Infrastructure;
using Carcass.Data.Entities;

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
            context.SaveChanges();
            InitializeMembership(context); 
        }

        public static void InitializeMembership(DatabaseContext context)
        {
            lock (_initMembershipLock)
            {
                // Reset WebSecurity internal state - hack, but there is no other way
                ResetWebSecurityInitialization();

                WebSecurity.InitializeDatabaseConnection(
                    "DefaultConnection",
                    "Users",
                    "UserEntityId",
                    "UserName",
                    autoCreateTables: true);

                if (!Roles.RoleExists(AppConstants.AdministratorsGroup))
                    Roles.CreateRole(AppConstants.AdministratorsGroup);
                if (!Roles.RoleExists(AppConstants.UsersGroup))
                    Roles.CreateRole(AppConstants.UsersGroup);

                // setup default administrator
                var adminName = "artem.kustikov@gmail.com";
                var provider = "google";
                var providerUserId = "https://www.google.com/accounts/o8/id?id=AItOawkHJByjLNYE2yHy5q2BXksDZVX0yZ2oj_g";
                if (!WebSecurity.UserExists(adminName))
                {

                    context.Users.Add(new UserEntity { UserName = adminName, DateRegistered = DateTime.UtcNow });
                    context.SaveChanges();
                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, adminName);

                    Roles.AddUserToRole(adminName, AppConstants.AdministratorsGroup);
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