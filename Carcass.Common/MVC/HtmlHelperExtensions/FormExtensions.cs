using System;
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
using Carcass.Resources;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static partial class FormExtensions
    {
        private static object _formLock = new object();

        private const string FormIndexKey = "LastFormId";

        public const string ValidationAttributeRequired = "data-val-required";

        public const string BootsrapClassError = "text-error";

        public const string BootsrapFormClassHorisontal = "form-horizontal";

        public const string BootsrapFormFieldClass = "control-group";

        public const string BootsrapFormFieldControlsClass = "controls";

        public const string BootsrapFormLabelClass = "control-label";

        public static FormContext GetFormContextForClientValidation(this HtmlHelper html)
        {
            
            if (!html.ViewContext.ClientValidationEnabled)
                return null;
            else
                return html.ViewContext.FormContext;
        }

        /// <summary>
        /// Writes an opening <form> tag to the response. When the user submits the form,
        //  the request will be processed by an action method.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="formClass">CSS class for <c>form</c> element</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <returns>An opening <form> tag</returns>
        public static CarcassMvcForm CarcassBeginForm(this HtmlHelper htmlHelper, string formClass = BootsrapFormClassHorisontal, FormMethod method = FormMethod.Post)
        {
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            return CarcassBeginFormImpl(htmlHelper, rawUrl, formClass, method);
        }

        /// <summary>
        /// Writes an opening <form> tag to the response. When the user submits the form,
        //  the request will be processed by an action method.
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
            FormMethod method, 
            IDictionary<string, object> htmlAttributes)
        {
            var formAction = UrlHelper.GenerateUrl(
                null, 
                actionName, 
                controllerName, 
                routeValues, 
                htmlHelper.RouteCollection, 
                htmlHelper.ViewContext.RequestContext, 
                true);
            return CarcassBeginFormImpl(htmlHelper, formAction, BootsrapFormClassHorisontal, method, htmlAttributes);
        }

        public static void CarcassEndForm(ViewContext viewContext)
        {
            viewContext.Writer.Write("</form>");
            viewContext.OutputClientValidation();
            viewContext.FormContext = null;
        }

        private static CarcassMvcForm CarcassBeginFormImpl(
            this HtmlHelper htmlHelper, 
            string formAction,
            string formCssClass, 
            FormMethod method, 
            IDictionary<string, object> htmlAttributes = null)
        {
            var tb = new TagBuilder("form");

            tb.AddCssClass(formCssClass);
            tb.MergeAttribute("action", formAction);
            tb.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            if (htmlAttributes != null)
                tb.MergeAttributes<string, object>(htmlAttributes);

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
