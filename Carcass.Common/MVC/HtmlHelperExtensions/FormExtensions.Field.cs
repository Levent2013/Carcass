using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Carcass.Common.Utility;
using Carcass.Common.Resources;
using Carcass.Common.MVC.HtmlHelperExtensions.Infrastructure;
using Carcass.Common.MVC.Extensions;

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
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="editorAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static IHtmlString CarcassFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            object editorAttributes = null)
        {
            Throw.IfNullArgument(expression, "expression");
            return CarcassFieldFor(html,
                expression,
                ExpressionHelper.GetExpressionText((LambdaExpression)expression),
                false,
                (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(editorAttributes));
        }

        /// <summary>
        /// Format HTML layout for form field (label + appropriate input)
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="inlineValidation">If <c>true</c> then control validation message will be rendered (CarcassValidationMessageFor)</param>
        /// <param name="editorAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static IHtmlString CarcassFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            bool inlineValidation,
            object editorAttributes = null)
        {
            Throw.IfNullArgument(expression, "expression");

            var htmlAttributes = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(editorAttributes);

            return CarcassFieldFor(html,
                expression,
                ExpressionHelper.GetExpressionText((LambdaExpression)expression),
                inlineValidation,
                htmlAttributes);
        }

        /// <summary>
        /// Format HTML layout for form field (label + appropriate input)
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="htmlFieldName">Model field name</param>
        /// <param name="inlineValidation">If <c>true</c> then control validation message will be rendered (CarcassValidationMessageFor)</param>
        /// <param name="editorAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static IHtmlString CarcassFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            string htmlFieldName,
            bool inlineValidation = false,
            object editorAttributes = null,
            object validationAttributes = null)
        {
            return CarcassFieldFor(html, 
                expression,
                htmlFieldName,
                inlineValidation,
                (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(editorAttributes),
                validationAttributes);
        }

        /// <summary>
        /// Format HTML layout for form field (label + appropriate input)
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the property to display.</param>
        /// <param name="htmlFieldName">Model field name</param>
        /// <param name="editorAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static IHtmlString CarcassFieldFor<TModel, TValue>(
            this HtmlHelper<TModel> html, 
            Expression<Func<TModel, TValue>> expression,
            string htmlFieldName,
            bool inlineValidation,
            IDictionary<string, object> editorAttributes,
            object validationAttributes = null)
        {
            Throw.IfNullArgument(expression, "expression");
            var formContext = html.ViewContext.FormContext;
            var formClass = (formContext is CarcassMvcFormContext)
                ? (formContext as CarcassMvcFormContext).FormClass
                : null;
            
            htmlFieldName = htmlFieldName ?? ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            var metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);

            return RenderCarcassFieldFor(html, metadata, htmlFieldName, formClass, editorAttributes, inlineValidation, validationAttributes);
        }

        /// <summary>
        /// Returns an HTML input element for each property in the object that is 
        /// represented by the <c>System.Linq.Expressions.Expression</c> expression. 
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
        /// <param name="htmlFieldName">A string that is used to disambiguate the names of HTML input elements that 
        ///                             are rendered for properties that have the same name.</param>
        /// <param name="templateName">The name of the template to use to render the object.</param>
        /// <returns>An HTML input element for each property in the object that is represented by the expression</returns>
        public static IHtmlString CarcassEditorFor<TModel, TValue>(
            HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            string htmlFieldName = null,
            string templateName = null,
            IDictionary<string, object> editorAttributes = null)
        {
            Throw.IfNullArgument(expression, "expression");

            return CarcassEditorFor(
                (HtmlHelper)html,
                (ModelMetadata)ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                templateName,
                htmlFieldName,
                editorAttributes);
        }

        public static IHtmlString CarcassEditorFor(
            this HtmlHelper html, 
            ModelMetadata metadata, 
            string templateName = null,
            string htmlFieldName = null,
            IDictionary<string, object> editorAttributes = null)
        {
            var formattedValue = metadata.Model;
            if (metadata.Model != null && !string.IsNullOrEmpty(metadata.EditFormatString))
                formattedValue = String.Format(CultureInfo.CurrentCulture, metadata.EditFormatString, metadata.Model);
            
            var htmlFieldPrefix = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            var localTemplates = new string[3] { templateName, metadata.TemplateHint, metadata.DataTypeName };

            var localViewData = new ViewDataDictionary(html.ViewDataContainer.ViewData)
            {
                Model = metadata.Model,
                ModelMetadata = metadata,
                TemplateInfo = new TemplateInfo()
                {
                    FormattedModelValue = formattedValue,
                    HtmlFieldPrefix = htmlFieldPrefix
                }
            };

            foreach (string folder in localTemplates)
            {
                if (String.IsNullOrEmpty(folder))
                    continue;

                var template = CarcassMvcSettings.TemplatesFolderEdit + "/" + folder;
                var partialView = ViewEngines.Engines.FindPartialView(html.ViewContext, template);
                if (partialView.View != null)
                {
                    using (var writer = new StringWriter(CultureInfo.InvariantCulture))
                    {
                        partialView.View.Render(new ViewContext(
                            html.ViewContext, 
                            partialView.View,
                            localViewData,
                            html.ViewContext.TempData, writer), writer);
                        
                        return MvcHtmlString.Create(writer.ToString());
                    }
                }
            }

            // template not found
            var typeName = metadata.ModelType.Name;
            if (metadata.IsNullableValueType && metadata.ModelType.GenericTypeArguments.Length > 0) 
            {
                typeName = metadata.ModelType.GenericTypeArguments[0].Name;
            }

            var fieldType = templateName ?? metadata.TemplateHint ?? metadata.DataTypeName ?? typeName;
            Infrastructure.EditorTemplates.ActionDelegate formatAction = Infrastructure.EditorTemplates.FindAction(fieldType); 

            if (formatAction == null)
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, ValidationResources.EditorTemplateNotFoundFor, fieldType));

            if (editorAttributes == null)
                editorAttributes = new Dictionary<string, object>();
            
            return formatAction(html, formattedValue, htmlFieldName, metadata, editorAttributes);
        }

        internal static IHtmlString RenderCarcassFieldFor<TModel>(
           HtmlHelper<TModel> html,
           ModelMetadata metadata,
           string htmlFieldName,
           string formClass,
           IDictionary<string, object> editorAttributes,
           bool inlineValidation = false,
           object validationAttributes = null)
        {
            Throw.IfNullArgument(metadata, "metadata");
            var isHorisontalForm = (formClass ?? String.Empty).HasCssClass(CarcassMvcSettings.BootsrapFormClassHorisontal);

            var labelAttributes = isHorisontalForm ? new Dictionary<string, object> { { "class", CarcassMvcSettings.BootsrapFormLabelClass } } : null;
            var label = LabelExtensions.FormatCarcassLabel((HtmlHelper)html, metadata, htmlFieldName, null, labelAttributes);
            var editor = html.CarcassEditorFor(metadata, null, htmlFieldName, editorAttributes);

            IHtmlString validation = null;
            if (inlineValidation)
            {
                var validationHtmlAttrs = validationAttributes == null
                    ? new Dictionary<string, object>() { { "class", CarcassMvcSettings.ValidationMessageClass } }
                    : (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(validationAttributes);

                if (!validationHtmlAttrs.ContainsKey("class"))
                    validationHtmlAttrs.Add("class", CarcassMvcSettings.ValidationMessageClass);

                validation = html.FieldValidationMessage(metadata, htmlFieldName, null, validationHtmlAttrs);
            }


            var fieldHtml = String.Empty;
            if (isHorisontalForm)
            {
                var tb = new TagBuilder("div");
                tb.AddCssClass(CarcassMvcSettings.BootsrapFormFieldClass);

                var tbControls = new TagBuilder("div");
                tbControls.AddCssClass(CarcassMvcSettings.BootsrapFormFieldControlsClass);
                tbControls.InnerHtml = editor.ToHtmlString() + " " + (validation != null ? validation.ToHtmlString() : String.Empty);

                var sb = new StringBuilder();
                sb.Append(label.ToHtmlString()).Append(tbControls.ToString(TagRenderMode.Normal));

                tb.InnerHtml = sb.ToString();
                fieldHtml = tb.ToString(TagRenderMode.Normal);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(label.ToHtmlString()).Append(editor.ToHtmlString());
                if(validation != null)
                    sb.Append(" ").Append(validation.ToHtmlString());
                
                fieldHtml = sb.ToString();
            }


            return MvcHtmlString.Create(fieldHtml);
        }
    }

}
