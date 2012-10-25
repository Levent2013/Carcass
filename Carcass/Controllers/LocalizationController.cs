using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Carcass.Infrastructure;
using Carcass.Common.Resources;
using Carcass.Common.Resources.Controls;

using MvcExtensions;

namespace Carcass.Controllers
{
    public class LocalizationController : Controller
    {
        [OutputCache(Duration=300, VaryByParam="*")]
        public ActionResult Controls(string culture)
        {
            CultureInfo locale = ParseCulture(culture);
            var sb = new StringBuilder();
            sb.Append(DatepickerResourcesLoader.GetResources(locale));
            sb.Append(TimepickerResourcesLoader.GetResources(locale));

            sb.AppendFormat(@"// {0} translation for bootbox
if(window.bootbox) bootbox.setLocale('{1}');", locale.Name, locale.TwoLetterISOLanguageName);
           
            return Content(sb.ToString(), "text/javascript");
        }

        [OutputCache(Duration = 300, VaryByParam = "*")]
        public ActionResult Validation(string culture)
        {
            CultureInfo locale = ParseCulture(culture);
            return Content(ValidationResourcesLoader.GetResources(locale), "text/javascript");
        }

        public ActionResult SetLanguage(string lang, string returnUrl)
        {
            var locale = ParseCulture(lang);

            var cookie = Response.Cookies[AppConstants.LanguageCookie] 
                ?? new HttpCookie(AppConstants.LanguageCookie);
            
            cookie.Value = locale.TwoLetterISOLanguageName;
            Response.Cookies.Set(cookie);

            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");
            
            return Redirect(returnUrl);
        }

        private static CultureInfo ParseCulture(string culture)
        {
            if (String.IsNullOrWhiteSpace(culture))
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            return CultureInfo.GetCultureInfo(culture) ?? Thread.CurrentThread.CurrentUICulture;
        }
    }
}
