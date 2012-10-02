using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Carcass.Common.Utility;
using Carcass.Common.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static partial class FormExtensions
    {
        /// <summary>
        /// Format HTML layout for form field in display mode
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <returns></returns>
        public static MvcHtmlString CarcassDisplayFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            Throw.IfNullArgument(expression, "expression");

            var metadata = (ModelMetadata)ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (metadata.ConvertEmptyStringToNull && ((object)String.Empty).Equals(metadata.Model))
                metadata.Model = (object)null;

            object value = metadata.Model;
            if (metadata.Model == null)
                value = (object)metadata.NullDisplayText;


            return MvcHtmlString.Empty;
        }

    }
}
