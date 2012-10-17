using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using MvcExtensions;

namespace Carcass.Infrastructure.Tasks
{
    public class RegisterRoutes : RegisterRoutesBase
    {
        public RegisterRoutes(RouteCollection routes)
            : base(routes)
        {
        }

        protected override void Register()
        {
            Routes.IgnoreRoute("favicon.ico");
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            Routes.MapRoute(
                "UserBlog",
                "Example/UserBlog/{id}-{username}",
                new { controller = "Example", action = "UserBlog", username = UrlParameter.Optional },
                new { id = @"\d+" }
            );

            Routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}