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
using Carcass.Common.Data.Extensions;
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

        protected IQueryBuilder Query { get; set; }

        public ActionResult Manage(ManageMessageId? message)
        {
            if (TempData["ManageMessageId"] != null)
                message = (ManageMessageId)TempData["ManageMessageId"];

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.ExternalLoginAdded ? "The external login was added."
                : message == ManageMessageId.ExternalLoginForProviderAlreadyAdded ? "The external login for this provider is already added."
                : message == ManageMessageId.YourProfileUpdated ? "Your profile has been updated successfully."
                : null;

            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");

            var user = Query.Find<UserEntity>(WebSecurity.CurrentUserId).MapTo<UserProfile>();
            return View(user);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(UserProfile model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                return View(model);
            }

            // TODO: implement Save() operation
            var entity = Query.Find<UserEntity>(model.Id);
            
            return RedirectToAction("Manage", new { Message = ManageMessageId.YourProfileUpdated });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManagePassword(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View("ManagePassword", model);
        }


        [AuthorizeWithMessage("You must be logged in as administrator to manage users", 
            Roles = Carcass.Infrastructure.AppConstants.AdministratorsGroup,
            Order = 1)]
        public ActionResult Users()
        {
            var users = Query.For<User>();
            return View("DisplayList", users);
        }

    }
}
