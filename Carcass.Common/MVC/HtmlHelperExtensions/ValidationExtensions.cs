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
    /// <summary>
    /// Implementation details got from MVC sources.
    /// <c>System.Web.Mvc.Html.ValidationExtensions</c>
    /// </summary>
    public static class ValidationExtensions
    {
        public static MvcHtmlString CarcassValidationSummary(this HtmlHelper htmlHelper,
            string cssClass,
            bool excludePropertyErrors = false)
        {
            return CarcassValidationSummary(
                htmlHelper,
                excludePropertyErrors, 
                null, 
                new Dictionary<string, object> { { "class", (object)cssClass }});
        }

        public static MvcHtmlString CarcassValidationSummary(this HtmlHelper htmlHelper, 
            bool excludePropertyErrors = false, 
            string message = null, 
            IDictionary<string, object> htmlAttributes = null)
        {
            Throw.IfNullArgument(htmlHelper, "htmlHelper");

            var clientValidation = htmlHelper.GetFormContextForClientValidation();
            var modelValid = htmlHelper.ViewData.ModelState.IsValid;
            if (modelValid)
            {
                if (clientValidation == null)
                    return null;
                else if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled && excludePropertyErrors)
                    return null;
            }

            var alert = new TagBuilder("div");
            if (htmlAttributes != null)
                alert.MergeAttributes<string, object>(htmlAttributes);

            alert.AddCssClass("alert alert-danger");
            var content = new StringBuilder();
            
            if(clientValidation == null) 
            {
                // add 'x' button
                var btn = new TagBuilder("button");
                btn.Attributes.Add("type", "button");
                btn.Attributes.Add("class", "close");
                btn.Attributes.Add("data-dismiss", "alert");
                btn.SetInnerText("×");

                content.Append(btn.ToString(TagRenderMode.Normal)).Append(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(message))
            {
                var tb = new TagBuilder("h5");
                tb.SetInnerText(message);
                content.Append(tb.ToString(TagRenderMode.Normal)).Append(Environment.NewLine);
            }


            var errorsCount = 0;
            content.Append("<ul>");
            foreach (ModelState modelState in ValidationExtensions.GetModelStateList(htmlHelper, excludePropertyErrors))
            {
                foreach (ModelError error in (Collection<ModelError>)modelState.Errors)
                {
                    var msg = ValidationExtensions.LoadUserErrorMessage(
                        htmlHelper.ViewContext.HttpContext, 
                        error, 
                        (ModelState)null);

                    if (!String.IsNullOrEmpty(msg))
                    {
                        ++errorsCount;
                        var li = new TagBuilder("li");
                        li.SetInnerText(msg);
                        content.AppendLine(li.ToString(TagRenderMode.Normal));
                    }
                }
            }

            content.Append("</ul>");
            
            alert.InnerHtml = content.ToString();
            
            // support legacy MVC client side validation
            alert.AddCssClass(modelValid || errorsCount == 0 ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            
            if (clientValidation != null)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    if (!excludePropertyErrors)
                        alert.MergeAttribute("data-valmsg-summary", "true");
                }
                else
                {
                    alert.GenerateId("validationSummary");
                    clientValidation.ValidationSummaryId = alert.Attributes["id"];
                    clientValidation.ReplaceValidationSummary = !excludePropertyErrors;
                }
            }
            
            return new MvcHtmlString(alert.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Returns the HTML markup for a validation-error message for each data field
        ///     that is represented by the specified expression, using the specified message.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the properties to render.</param>
        /// <param name="validationMessage">The message to display if the specified field contains an error.</param>
        /// <returns> If the property or object is valid, an empty string; 
        ///         otherwise, a span elementthat contains an error message.
        /// </returns>
        public static MvcHtmlString CarcassValidationMessageFor<TModel, TProperty>(
            this HtmlHelper<TModel> html, 
            Expression<Func<TModel, TProperty>> expression, 
            string validationMessage = null,
            IDictionary<string, object> htmlAttributes = null)
        {

            var metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText((LambdaExpression) expression);
            return FieldValidationMessage(html, metadata, htmlFieldName, validationMessage, htmlAttributes);
        }

        internal static MvcHtmlString FieldValidationMessage(
            this HtmlHelper htmlHelper, 
            ModelMetadata metadata, 
            string htmlFieldName, 
            string validationMessage = null, 
            IDictionary<string, object> htmlAttributes = null)
        {
            Throw.IfNullArgument(metadata, "metadata");
            
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            var clientValidation = htmlHelper.GetFormContextForClientValidation();
            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullHtmlFieldName) && clientValidation == null)
                return (MvcHtmlString)null;
            
            var modelState = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            var modelErrorCollection = modelState == null ? null : modelState.Errors;
            var error = modelErrorCollection == null || modelErrorCollection.Count == 0
                ? null
                : modelErrorCollection.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage)) ?? modelErrorCollection[0];
            
            if (error == null && clientValidation == null)
                return (MvcHtmlString)null;
            
            var tagBuilder = new TagBuilder("span");
            tagBuilder.MergeAttributes<string, object>(htmlAttributes);
            tagBuilder.AddCssClass(error != null ? HtmlHelper.ValidationMessageCssClassName : HtmlHelper.ValidationMessageValidCssClassName);
            
            if (!String.IsNullOrEmpty(validationMessage))
                tagBuilder.SetInnerText(validationMessage);
            else if (error != null)
                tagBuilder.SetInnerText(LoadUserErrorMessage(htmlHelper.ViewContext.HttpContext, error, modelState));

            if (clientValidation != null)
            {
                var isEmpty = String.IsNullOrEmpty(validationMessage);
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    tagBuilder.MergeAttribute("data-valmsg-for", fullHtmlFieldName);
                    tagBuilder.MergeAttribute("data-valmsg-replace", isEmpty ? "true" : "false");
                }
                else
                {
                    tagBuilder.GenerateId(fullHtmlFieldName + "_validationMessage");

                    var validationMetadata = htmlHelper.ViewContext.FormContext.GetValidationMetadataForField(fullHtmlFieldName, true);
                    foreach (ModelClientValidationRule rule in 
                        Enumerable.SelectMany<ModelValidator, ModelClientValidationRule>(
                            ModelValidatorProviders.Providers.GetValidators(metadata, (ControllerContext) htmlHelper.ViewContext), 
                                v => v.GetClientValidationRules()))
                    {
                        validationMetadata.ValidationRules.Add(rule);
                    }
      
                    validationMetadata.ReplaceValidationMessageContents = isEmpty;
                    validationMetadata.ValidationMessageId = tagBuilder.Attributes["id"];
                }
            }
            
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        private static string LoadUserErrorMessage(HttpContextBase httpContext, ModelError error, ModelState modelState)
        {
            if (error != null && !String.IsNullOrEmpty(error.ErrorMessage))
                return error.ErrorMessage;
            
            else if (modelState == null)
            {
                return null;
            }
            else
            {
                var value = modelState.Value != null ? modelState.Value.AttemptedValue : ValidationResources.ValueNull;
                return String.Format(
                    CultureInfo.CurrentCulture, 
                    ValidationResources.InvalidPropertyValue, 
                    value);
            }
        }

        private static IEnumerable<ModelState> GetModelStateList(HtmlHelper htmlHelper, bool excludePropertyErrors)
        {
            if (excludePropertyErrors)
            {
                ModelState modelState;
                htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out modelState);
                if (modelState != null)
                    return (IEnumerable<ModelState>)new ModelState[] { modelState };
                else
                    return (IEnumerable<ModelState>)new ModelState[0];
            }
            else
            {
                var ordering = new Dictionary<string, int>();
                var modelMetadata = htmlHelper.ViewData.ModelMetadata;
                if (modelMetadata != null)
                {
                    foreach (ModelMetadata prop in modelMetadata.Properties)
                    {
                        ordering[prop.PropertyName] = prop.Order;
                    }
                }
                
                return Enumerable.Select( (IEnumerable<KeyValuePair<string, ModelState>>)htmlHelper.ViewData.ModelState, 
                                           kv => new { kv = kv, name = kv.Key})
                              .OrderBy (p => ordering.Get(p.name, 10000))
                              .Select (p => p.kv.Value);
            }
        }

    }
}
