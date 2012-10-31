using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using System.Globalization;
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
using Carcass.Common.MVC.Metadata;
using Carcass.Common.MVC.Security;
using Carcass.Common.MVC.Extensions;
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

            Query.Lookup<UserProfile>(model).Save();
            TempData["ManageMessageId"] = ManageMessageId.YourProfileUpdated;
            return RedirectToAction("Manage");
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
                        TempData["ManageMessageId"] = ManageMessageId.ChangePasswordSuccess;
                        return RedirectToAction("Manage");
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
                        TempData["ManageMessageId"] = ManageMessageId.SetPasswordSuccess;
                        return RedirectToAction("Manage");
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
            var users = Query.For<Carcass.Models.User>();
            return View("DisplayList", users.ToList());
        }

        [ChildActionOnly, CarcassSelectListAction]
        // [OutputCache(VaryByParam="selected", Duration=1800)] 
        public ActionResult TimeZones(string selected, string name /* HTML control name */)
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var items = new List<SelectListItem>();
            
            foreach (TimeZoneInfo timeZone in timeZones)
            {
                var offset = (int)timeZone.BaseUtcOffset.TotalMinutes;
                items.Add(new SelectListItem
                {
                    Text = String.Format("{0} ({1})", timeZone.DisplayName, timeZone.Id),
                    Value = timeZone.Id,
                    Selected = timeZone.Id.Equals(selected)
                });
            }

            // Direct call could be used:
            // return Content(Carcass.Common.MVC.Html.HtmlRenderer.Dropdown(name, items).ToString());

            return View("DropDownList", new SelectList(items, "Value", "Text", selected));
        }
    }
}
