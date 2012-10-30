using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Carcass.Common.Collections.Extensions;
using Carcass.Common.Utility;
using Carcass.Common.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static partial class FormExtensions
    {
        private static object _formLock = new object();

        private const string FormIndexKey = "LastFormId";

        public static FormContext GetFormContextForClientValidation(this HtmlHelper html)
        {
            
            if (!html.ViewContext.ClientValidationEnabled)
                return null;
            else
                return html.ViewContext.FormContext;
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form,
        /// the request will be processed by an action method.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="formClass">CSS class for <c>form</c> element</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An opening <form> tag</returns>
        public static CarcassMvcForm CarcassBeginForm(this HtmlHelper htmlHelper,
            string formClass = CarcassMvcSettings.BootsrapFormClassHorisontal, 
            FormMethod method = FormMethod.Post,
            object htmlAttributes = null)
        {
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            return CarcassBeginFormImpl(htmlHelper, rawUrl, formClass, method, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form,
        /// the request will be processed by an action method.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues"> An object that contains the parameters for a route. 
        ///         The parameters are retrievedthrough reflection by examining the properties of the object. 
        ///         This objectis typically created by using object initializer syntax.
        /// </param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An opening <form> tag</returns>
        public static CarcassMvcForm CarcassBeginForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues,
            object htmlAttributes = null)
        {

            return CarcassBeginForm(
                htmlHelper,
                actionName,
                controllerName,
                routeValues,
                FormMethod.Post,
                (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form,
        /// the request will be processed by an action method.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An opening <form> tag</returns>
        public static CarcassMvcForm CarcassBeginForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            FormMethod method = FormMethod.Post,
            IDictionary<string, object> htmlAttributes = null)
        {
            var formAction = UrlHelper.GenerateUrl(
                null,
                actionName,
                controllerName,
                null,
                htmlHelper.RouteCollection,
                htmlHelper.ViewContext.RequestContext,
                true);

            return CarcassBeginFormImpl(
                htmlHelper, 
                formAction, 
                CarcassMvcSettings.BootsrapFormClassHorisontal, 
                method, 
                htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response. When the user submits the form,
        /// the request will be processed by an action method.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues"> An object that contains the parameters for a route. 
        ///         The parameters are retrievedthrough reflection by examining the properties of the object. 
        ///         This objectis typically created by using object initializer syntax.
        /// </param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An opening <form> tag</returns>
        public static CarcassMvcForm CarcassBeginForm(
            this HtmlHelper htmlHelper, 
            string actionName, 
            string controllerName,
            object routeValues, 
            FormMethod method = FormMethod.Post, 
            IDictionary<string, object> htmlAttributes = null)
        {
            var formAction = UrlHelper.GenerateUrl(
                null, 
                actionName, 
                controllerName,
                HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues), 
                htmlHelper.RouteCollection, 
                htmlHelper.ViewContext.RequestContext, 
                true);
            return CarcassBeginFormImpl(htmlHelper, formAction, CarcassMvcSettings.BootsrapFormClassHorisontal, method, htmlAttributes);
        }

        /// <summary>
        /// Format HTML layout for form field (label + appropriate input)
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="form">Current Carcass form</param>
        /// <param name="htmlFieldName">Model field name</param>
        /// <param name="editorAttributes">An object that contains the HTML attributes to set for the input.</param>
        /// <returns></returns>
        public static IHtmlString CarcassFormActions(
            this HtmlHelper html,
            IDictionary<string, object> buttons,
            string groupClass = CarcassMvcSettings.BootsrapFormActionsClass)
        {
            Throw.IfNullArgument(buttons, "buttons");
            
            var tb = new TagBuilder("div");
            tb.AddCssClass(groupClass);

            var sb = new StringBuilder();
            foreach (var btn in buttons)
            {
                var btnBuilder = new TagBuilder("input");
                btnBuilder.MergeAttribute("value", btn.Key);
                
                var attrs = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(btn.Value);
                btnBuilder.MergeAttribute("type", attrs.Get("type", "button"));
                btnBuilder.AddCssClass("btn " + attrs.Get("class", String.Empty));
                
                sb.Append(btnBuilder.ToString(TagRenderMode.SelfClosing));
            }


            tb.InnerHtml = sb.ToString();

            return MvcHtmlString.Create(tb.ToString(TagRenderMode.Normal));
        }

        public static void CarcassEndForm(ViewContext viewContext)
        {
            viewContext.Writer.Write("</form>");
            viewContext.OutputClientValidation();
            viewContext.FormContext = null;
        }


        /// <summary>
        /// Returns an HTML input element for each property in the model.
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="options">
        /// Form rendering options.
        /// Currently supported following options:
        /// <list type="table">
        ///     <item><term>InlineValidation</term><description>Write validation messages for each field</description>
        ///     </item>
        ///     <item><term>ValidationClass</term><description>CSS class for validation messages </description></item>
        /// </list>
        /// </param>
        /// <returns>An HTML input element for each property in the model.</returns>
        public static IHtmlString CarcassEditorForModel(this HtmlHelper html, object options = null)
        {
            return CarcassEditorFor(
                html, 
                html.ViewData.ModelMetadata, 
                "Object", 
                html.ViewData.ModelMetadata.PropertyName,
                options != null ? (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(options) : null);
        }


        private static CarcassMvcForm CarcassBeginFormImpl(
            this HtmlHelper htmlHelper, 
            string formAction,
            string formCssClass, 
            FormMethod method, 
            IDictionary<string, object> htmlAttributes = null)
        {
            var tb = new TagBuilder("form");

            tb.MergeAttribute("action", formAction);
            tb.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            if (htmlAttributes != null)
                tb.MergeAttributes<string, object>(htmlAttributes);
            tb.AddCssClass(formCssClass);
            
            var generateFormId = htmlHelper.ViewContext.ClientValidationEnabled && !htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled;
            if (generateFormId)
                tb.GenerateId(GenerateFormId(htmlHelper.ViewContext.HttpContext));

            htmlHelper.ViewContext.Writer.Write(tb.ToString(TagRenderMode.StartTag));
            var mvcForm = new CarcassMvcForm(htmlHelper.ViewContext, formCssClass);
            if (generateFormId)
                htmlHelper.ViewContext.FormContext.FormId = tb.Attributes["id"];

            return mvcForm;
        }

        private static string GenerateFormId(HttpContextBase context)
        {
            Throw.IfNullArgument(context, "context");
            lock (_formLock)
            {
                var formId = 0;
                var lastId = context.Items[FormIndexKey];
                if (lastId == null || !(lastId is int))
                {
                    formId = 1;
                }
                else
                {
                    formId = ((int)lastId) + 1;
                }


                context.Items[FormIndexKey] = formId;
                return formId.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
