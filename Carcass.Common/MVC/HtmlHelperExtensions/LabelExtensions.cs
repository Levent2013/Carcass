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
    public static partial class LabelExtensions
    {
        /// <summary>
        /// Returns an HTML label element and the property name of the property that
        /// is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An formatted HTML label element</returns>
        public static IHtmlString CarcassLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            Throw.IfNullArgument(expression, "expression");

            return LabelExtensions.FormatCarcassLabel(
                (HtmlHelper)html,
                ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData),
                ExpressionHelper.GetExpressionText((LambdaExpression) expression),
                null,
                (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Gets the description given in DisplayName attribute.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the description.</param>
        /// <returns>The description for the model.</returns>
        public static IHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
            var description = metadata.Description;
            if (!String.IsNullOrEmpty(description))
                return new MvcHtmlString(description);

            return html.DisplayNameFor(expression);
        }

        /// <summary>
        /// Returns an HTML label element and the property name of the property that
        /// is represented by the specified expression.
        /// </summary>
        /// <param name="html">The HTML helper instance.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="labelText">Custom label text to rewrite one from control metadata</param>
        /// <param name="htmlFieldName">Model field name</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <param name="metadataProvider">The metadata provider</param>
        /// <returns></returns>
        internal static IHtmlString FormatCarcassLabel(
            HtmlHelper html, 
            ModelMetadata metadata,
            string htmlFieldName,
            string labelText = null, 
            IDictionary<string, object> htmlAttributes = null)
        {
            Throw.IfNullArgument(metadata, "metadata");
            Throw.IfBadArgument(() => String.IsNullOrEmpty(htmlFieldName), "HTML field name is undefined");
            
            if (labelText == null)
            {
                labelText = LoadFieldName(metadata, htmlFieldName);
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }
            else
            {
                var fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);

                var tg = new TagBuilder("label");
                tg.AddCssClass("control-label");
                tg.Attributes.Add("for", TagBuilder.CreateSanitizedId(fullHtmlFieldName));

                if (metadata.IsRequired)
                {
                    string requredMessage = null;
                    var validationAttributes = html.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata);
                    html.ViewContext.FormContext.RenderedField(fullHtmlFieldName, false);
                    if (validationAttributes.ContainsKey(CarcassMvcSettings.ValidationAttributeRequired))
                        requredMessage = validationAttributes[CarcassMvcSettings.ValidationAttributeRequired] as string;
                    if (requredMessage == null)
                        requredMessage = String.Format(ValidationResources.PropertyValueRequired, labelText);
                    
                    TagBuilder star = new TagBuilder("span");
                    star.SetInnerText("*");
                    star.AddCssClass(CarcassMvcSettings.BootsrapClassError);
                    star.MergeAttribute("title", requredMessage);
                    tg.InnerHtml = html.Encode(labelText) + "&nbsp;" + star.ToString(TagRenderMode.Normal);
                }
                else
                {
                    tg.SetInnerText(labelText);
                }

                if (htmlAttributes != null)
                    tg.MergeAttributes(htmlAttributes);
                
                return new MvcHtmlString(tg.ToString(TagRenderMode.Normal));
            }
            
        }

        internal static string LoadFieldName(ModelMetadata metadata, string htmlFieldName)
        {
            var displayName = metadata.DisplayName;
            if (displayName == null)
            {
                var propertyName = metadata.PropertyName;
                if (propertyName == null)
                    displayName = Enumerable.Last<string>((IEnumerable<string>)htmlFieldName.Split(".".ToCharArray()));
                else
                    displayName = propertyName;
            }

            return displayName;
        }


    }
}
