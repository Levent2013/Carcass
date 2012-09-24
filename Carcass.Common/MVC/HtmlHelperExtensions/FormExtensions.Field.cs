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
using Carcass.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static partial class FormExtensions
    {
        /// <summary>
        /// Format HTML layout for form field (label + appropriate input)
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="form">Current Carcass form</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static MvcHtmlString CarcassFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html, 
            CarcassMvcForm form,
            Expression<Func<TModel, TValue>> expression)
        {
            Throw.IfNullArgument(form, "form");
            Throw.IfNullArgument(expression, "expression");

            var isHorisontalForm = form.FormClass == FormExtensions.BootsrapFormClassHorisontal;
            var labelAttributes = isHorisontalForm ? new { @class = BootsrapFormLabelClass } : null;
            var label = html.CarcassLabelFor(expression, labelAttributes);
            var editor = html.EditorFor(expression);


            var fieldHtml = String.Empty;
            if (isHorisontalForm)
            {
                var tb = new TagBuilder("div");
                tb.AddCssClass(BootsrapFormFieldClass);
                                
                var tbControls = new TagBuilder("div");
                tbControls.AddCssClass(BootsrapFormFieldControlsClass);
                tbControls.InnerHtml = editor.ToHtmlString();
                
                var sb = new StringBuilder();
                sb.Append(label.ToHtmlString()).Append(tbControls.ToString(TagRenderMode.Normal));
                
                tb.InnerHtml = sb.ToString();
                fieldHtml = tb.ToString(TagRenderMode.Normal);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(label.ToHtmlString()).Append(editor.ToHtmlString());
                fieldHtml = sb.ToString();
            }

            return MvcHtmlString.Create(fieldHtml);
        }
    }
}
