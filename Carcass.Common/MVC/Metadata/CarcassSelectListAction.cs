using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Carcass.Common.MVC.Metadata
{
    /// <summary>
    /// Got from MvcExtensions.SelectListActionAttribute and redesigned
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CarcassSelectListAction : FilterAttribute, IActionFilter
    {
        private const string DefaultArgumentName = "selected";

        private const string FieldArgumentName = "name";

        /// <summary>
        /// Get or sets the name of argument which is used to pass selected value into the action method. 
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="context">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ViewResultBase)
            {
                var result = (ViewResultBase)context.Result;
                CopyViewDataProperties(context.ParentActionViewContext.ViewData, result.ViewData);
            }
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="context">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var viewData = context.ParentActionViewContext.ViewData;
            context.ActionParameters[ArgumentName ?? DefaultArgumentName] = GetSelectedValue(context);

            context.ActionParameters[FieldArgumentName] = viewData.TemplateInfo.HtmlFieldPrefix;
        }

        private static void CopyViewDataProperties(ViewDataDictionary source, ViewDataDictionary destination)
        {
            destination.ModelMetadata = source.ModelMetadata;
            destination.TemplateInfo.FormattedModelValue = source.TemplateInfo.FormattedModelValue;
            destination.TemplateInfo.HtmlFieldPrefix = source.TemplateInfo.HtmlFieldPrefix;
        }

        private static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> predicate =
                t =>
                {
                    if (t.IsGenericType)
                    {
                        return t.GetGenericTypeDefinition() == interfaceType;
                    }

                    return false;
                };

            if (predicate(queryType))
            {
                return queryType;
            }

            return queryType.GetInterfaces().FirstOrDefault(predicate);
        }

        private static object GetAttemptedValue(ViewDataDictionary viewData)
        {
            ModelState modelState;
            if (viewData.ModelState.TryGetValue(viewData.ModelMetadata.PropertyName, out modelState) 
                    && modelState.Value != null)
            {
                return modelState.Value.ConvertTo(GetModelType(viewData.ModelMetadata.ModelType), null);
            }

            return null;
        }

        private static Type GetModelType(Type type)
        {
            if (type.IsArray || type == typeof(string))
            {
                return type;
            }

            var enumerableType = ExtractGenericInterface(type, typeof(IEnumerable<>));
            if (enumerableType == null)
            {
                return type;
            }

            var elementType = enumerableType.GetGenericArguments()[0];
            return elementType.MakeArrayType();
        }

        private static object GetSelectedValue(ControllerContext context)
        {
            var viewData = context.ParentActionViewContext.ViewData;
            return GetAttemptedValue(viewData) ?? viewData.Model;
        }
    }
}
