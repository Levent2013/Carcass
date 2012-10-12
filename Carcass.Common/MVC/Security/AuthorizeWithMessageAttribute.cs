using System;

using System.Web.Mvc;

namespace Carcass.Common.MVC.Security
{
    public class AuthorizeWithMessageAttribute : AuthorizeAttribute
    {
        public const string MessageKey = "ExtendedAuthorizeAttribute_Message";

        public AuthorizeWithMessageAttribute()
        {
        }

        public AuthorizeWithMessageAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData[MessageKey] = Message;
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
