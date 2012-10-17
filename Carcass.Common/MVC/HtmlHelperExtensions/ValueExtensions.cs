using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Carcass.Common.Utility;
using Carcass.Common.Collections.Extensions;
using Carcass.Common.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static class ValueExtensions
    {
        public static string ToStringInvariant(this int number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }
}
