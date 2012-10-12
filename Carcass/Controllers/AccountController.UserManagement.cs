using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Autofac;
using Autofac.Integration.Mvc;

using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

using Carcass.Common.Data;
using Carcass.Common.MVC.Security;
using Carcass.Models;
using Carcass.Data;
using Carcass.Data.Entities;

using MvcExtensions;

namespace Carcass.Controllers
{
    public partial class AccountController
    {
        public AccountController(IQueryBuilder queryBuilder)
        {
            Query = queryBuilder;
        }

        private IQueryBuilder Query { get; set; }

        [AuthorizeWithMessage("You must be logged in as administrator to manage users", 
            Roles = Carcass.Infrastructure.AppConstants.AdministratorsGroup,
            Order = 1)]
        public ActionResult Users()
        {
            var users = Query.For<UserEntity>();

            return View("DisplayList", users);
        }

    }
}
