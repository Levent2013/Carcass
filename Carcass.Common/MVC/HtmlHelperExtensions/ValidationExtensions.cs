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

using Carcass.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    /// <summary>
    /// Implementation details got from MVC sources.
    /// <c>System.Web.Mvc.Html.ValidationExtensions</c>
    /// </summary>
    public static class ValidationExtensions
    {
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
                        var li = new TagBuilder("li");
                        li.SetInnerText(msg);
                        content.AppendLine(li.ToString(TagRenderMode.Normal));
                    }
                }
            }

            content.Append("</ul>");
            
            alert.InnerHtml = content.ToString();
            if (htmlAttributes != null)
                alert.MergeAttributes<string, object>(htmlAttributes);

            // support legacy MVC client side validation
            alert.AddCssClass(modelValid ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            
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
                var value = modelState.Value != null ? modelState.Value.AttemptedValue : Validation.ValueNull;
                return String.Format(
                    CultureInfo.CurrentCulture, 
                    Validation.InvalidPropertyValue, 
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
