using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
           
            return Content(sb.ToString(), "text/javascript");
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
