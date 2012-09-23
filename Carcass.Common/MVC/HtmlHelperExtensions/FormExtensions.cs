using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Carcass.Common.Utility;
using Carcass.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static partial class FormExtensions
    {
        public const string ValidationAttributeRequired = "data-val-required";

        public const string BootsrapCssClassError = "text-error";

        public static FormContext GetFormContextForClientValidation(this HtmlHelper html)
        {
            if (!html.ViewContext.ClientValidationEnabled)
                return null;
            else
                return html.ViewContext.FormContext;
        }

    }
}
