using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
    
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Carcass.Common.Utility;
using Carcass.Common.Collections.Extensions;
using Carcass.Common.Resources;
using Carcass.Common.MVC.Extensions;

using MvcExtensions;

namespace Carcass.Common.MVC.HtmlHelperExtensions.Infrastructure
{
    internal static class EditorTemplates
    {
        public const string DefaultDateFormat = "dd-mm-yyyy";

        /// <summary>
        /// 'h' - 12h hour, 'hh'- 24h hour, 'tt' - a/pm, mm - minutes 
        /// </summary>
        public const string DefaultTimeFormat = "hh:mm";

        // Number validation attributes
        public const string DataCarcassValidation = "data-carcass-val";
        public const string DataNumberDecimalSeparator = "data-num-decimal-separator";
        public const string DataNumberGroupSeparator = "data-num-group-separator";

        internal const string DateControlPostfix = ".Date";
        internal const string TimeControlPostfix = ".Time";

        public delegate IHtmlString ActionDelegate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata fieldMetadata, IDictionary<string, object> editorAttributes);

        private static readonly Dictionary<string, ActionDelegate> _defaultEditorActions
            = new Dictionary<string, ActionDelegate>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
        {
            { "HiddenInput", HiddenInputTemplate },
            { "MultilineText", MultilineTextTemplate },
            { "Password", PasswordTemplate },
            { "Text", StringTemplate },
            { "CreditCard", CreditCardTemplate },
            { "Currency", CurrencyTemplate },
            
            { "Html", HtmlTemplate },
            //{ "Collection", CollectionTemplate },
            { "Duration", FloatTemplate },
            
            { "PhoneNumber", PhoneNumberTemplate },
            { "Url", UrlTemplate },
            { "ImageUrl", UrlTemplate },
            { "EmailAddress", EmailAddressTemplate },
            { "PostalCode", PostalCodeTemplate },
            { "DateTime", DateTimeTemplate },
            { "Date", DateTemplate },
            { "Time", TimeTemplate },
            { "Upload", UploadTemplate },
            
            { "RenderAction", RenderActionTemplate },
            
            { typeof (sbyte).Name, SignedIntegerTemplate},
            { typeof (int).Name, SignedIntegerTemplate },
            { typeof (short).Name, SignedIntegerTemplate },
            { typeof (long).Name, SignedIntegerTemplate },
            
            { typeof (byte).Name, UnsignedIntegerTemplate },
            { typeof (ushort).Name, UnsignedIntegerTemplate },
            { typeof (uint).Name, UnsignedIntegerTemplate },
            { typeof (ulong).Name, UnsignedIntegerTemplate },
            
            { typeof (bool).Name, BooleanTemplate},
            { typeof (decimal).Name, FloatTemplate},
            { typeof (float).Name, FloatTemplate},
            { typeof (double).Name, FloatTemplate},
            
            { typeof (string).Name, StringTemplate},
            { typeof (object).Name, ObjectTemplate}
        };

        internal static ActionDelegate FindAction(string fieldType)
        {
            return _defaultEditorActions.ContainsKey(fieldType) 
                ? _defaultEditorActions[fieldType]
                : NotImplementedTemplate; 
        }

        internal static IHtmlString NotImplementedTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata modelMetadata,
            IDictionary<string, object> htmlAttributes)
        {
            return MvcHtmlString.Create(String.Format(
                "<div class=\"alert\">Editor for property \"{0}\" is not implemented</div>",
                htmlFieldName ?? modelMetadata.DisplayName ?? modelMetadata.PropertyName));
        }

        internal static IHtmlString ObjectTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName,
            ModelMetadata modelMetadata, 
            IDictionary<string, object> htmlAttributes)
        {
            var viewData = html.ViewContext.ViewData;
            var templateInfo = viewData.TemplateInfo;
            var containerData = html.ViewDataContainer.ViewData;

            var formContext = html.ViewContext.FormContext;
            var isHorisontalForm = (formContext is CarcassMvcFormContext) &&
                ((formContext as CarcassMvcFormContext).FormClass ?? String.Empty).HasCssClass (CarcassMvcSettings.BootsrapFormClassHorisontal);

            var sb = new StringBuilder();
            if (templateInfo.TemplateDepth > 1)
            {
                if (modelMetadata.Model != null)
                    return MvcHtmlString.Create(modelMetadata.SimpleDisplayText);
                else
                    return MvcHtmlString.Create(modelMetadata.NullDisplayText);
            }
            else
            {
                var inlineValidation = htmlAttributes.Get("InlineValidation", true);
                var validationClass = htmlAttributes.Get("ValidationClass", CarcassMvcSettings.ValidationMessageClass);
                var validationAttrs = new Dictionary<string, object>() { { "class", validationClass } };
                
                var editorAttributes = new Dictionary<string, object>();
                if(htmlAttributes.ContainsKey("EditorClass"))
                    editorAttributes.Add("class", htmlAttributes["EditorClass"]);
                
                var content = new TagBuilder("div");
                if (htmlAttributes.ContainsKey("CssClass"))
                    content.AddCssClass(htmlAttributes["CssClass"] as string);
                sb.Append(content.ToString(TagRenderMode.StartTag));

                foreach (var metadata in modelMetadata.Properties.Where(pm => EditorTemplates.ShouldShow(pm, templateInfo)))
                {
                    var fieldType = metadata.TemplateHint ?? metadata.DataTypeName;
                    var isHidden = fieldType == "HiddenInput";

                    if (!isHidden && !metadata.HideSurroundingHtml)
                    {
                        var labelAttributes = isHorisontalForm 
                            ? new Dictionary<string, object> { { "class", CarcassMvcSettings.BootsrapFormLabelClass } } : null;
                        var label = FormExtensions.FormatCarcassLabel(
                            (HtmlHelper)html, 
                            metadata,
                            metadata.PropertyName, 
                            null, 
                            labelAttributes);

                        if (isHorisontalForm)
                            sb.AppendFormat("<div class=\"{0}\">", CarcassMvcSettings.BootsrapFormFieldClass);
                        sb.Append(label.ToHtmlString());
                        if (isHorisontalForm)
                            sb.AppendFormat("<div class=\"{0}\">", CarcassMvcSettings.BootsrapFormFieldControlsClass);
                    }

                    // Reset ViewBag values
                    if (containerData.ContainsKey(metadata.PropertyName))
                        containerData.Remove(metadata.PropertyName);
                    
                    var fieldName = templateInfo.GetFullHtmlFieldName(metadata.PropertyName);
                    sb.Append(html.CarcassEditorFor(metadata, null, fieldName, editorAttributes.Clone()).ToHtmlString());

                    if (!isHidden && !metadata.HideSurroundingHtml)
                    {
                        if (inlineValidation)
                        {
                            var typeName = metadata.ModelType.Name;
                            if (metadata.IsNullableValueType && metadata.ModelType.GenericTypeArguments.Length > 0)
                                typeName = metadata.ModelType.GenericTypeArguments[0].Name;
                            
                            sb.Append(" ");
                            if (typeName == "DateTime")
                            {
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, metadata.PropertyName + DateControlPostfix, null, validationAttrs.Clone())
                                        .ToHtmlString()).Append(" ");
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, metadata.PropertyName + TimeControlPostfix, null, validationAttrs.Clone())
                                        .ToHtmlString()).Append(" ");
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, metadata.PropertyName, null, validationAttrs.Clone())
                                        .ToHtmlString());
                            }
                            else
                            {
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, metadata.PropertyName, null, validationAttrs.Clone())
                                    .ToHtmlString());
                            }
                        }

                        if (isHorisontalForm)
                            sb.Append("</div></div>\r\n");
                    }
                }

                sb.Append(content.ToString(TagRenderMode.EndTag));
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        internal static IHtmlString HiddenInputTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            Throw.IfNullArgument(metadata, "metadata");
            object value = formattedValue;
            var binary = value as Binary;
            if (binary != (Binary)null)
            {
                value = (object)Convert.ToBase64String(binary.ToArray());
            }
            else
            {
                byte[] inArray = value as byte[];
                if (inArray != null)
                    value = (object)Convert.ToBase64String(inArray);
            }

            return InputExtensions.Hidden(html, htmlFieldName, value, editorAttributes);
        }

        internal static IHtmlString MultilineTextTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            return TextAreaExtensions.TextArea(
                html, 
                htmlFieldName, 
                formattedValue as string,
                CarcassMvcSettings.TextAreaRows, 
                0, 
                MergeAttributes(editorAttributes, "multi-line"));
        }

        internal static IHtmlString HtmlTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            return TextAreaExtensions.TextArea(
                html,
                htmlFieldName,
                formattedValue as string, CarcassMvcSettings.HtmlTextAreaRows, 0,
                MergeAttributes(editorAttributes, "html-editor"));
        }

        internal static IHtmlString PasswordTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            // evaluate field value maxlength
            LoadMaxLength(html, metadata, editorAttributes);
            
            return InputExtensions.Password(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "password", "password"));
        }

        internal static IHtmlString StringTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            // evaluate field value maxlength
            LoadMaxLength(html, metadata, editorAttributes);
            
            return InputExtensions.TextBox(
                html,
                htmlFieldName, 
                formattedValue,
                editorAttributes);
        }

        internal static IHtmlString CurrencyTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            editorAttributes = MergeAttributes(editorAttributes, "currency");
            MergeCurrencyAttributes (editorAttributes);
            return InputExtensions.TextBox(html, htmlFieldName, formattedValue, editorAttributes);
        }

        internal static IHtmlString RenderActionTemplate(
            HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var settings = metadata.GetRenderActionSetting();

            Throw.IfNullArgument(settings, "RenderAction settings not found");
            Throw.IfNullArgument(settings.Action, "RenderAction is not initialized");

            var htmlFieldPrefix = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            var viewData = new ViewDataDictionary(html.ViewData)
            {
                Model = metadata.Model,
                ModelMetadata = metadata,
                TemplateInfo = new TemplateInfo()
                {
                    FormattedModelValue = formattedValue,
                    HtmlFieldPrefix = htmlFieldPrefix
                }
            };

            ViewContext viewContext = new ViewContext(
                (ControllerContext)html.ViewContext, 
                html.ViewContext.View, 
                viewData, 
                html.ViewContext.TempData,
                html.ViewContext.Writer);

            var htmlLocal = new HtmlHelper(viewContext, new ViewDataContainer(viewData));

            return settings.Action(htmlLocal);
        }

        internal static IHtmlString UploadTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            var inputAttributes = MergeAttributes(editorAttributes, null, "file");
            
            var property = metadata.ContainerType.GetProperty(
                metadata.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            Throw.IfNullArgument(property, "Property {0} not found in {1}", metadata.PropertyName, metadata.ContainerType.Name);

            var uploadAttr = property.GetCustomAttribute<DataAnnotations.FileUploadAttribute>();
            if(uploadAttr != null) {
                if (uploadAttr.Multiple && !inputAttributes.ContainsKey("multiple"))
                    inputAttributes.Add("multiple", "multiple");
            }

            var isMultiple = inputAttributes.ContainsKey("multiple");
            var input = InputExtensions.TextBox(
                html,
                htmlFieldName,
                formattedValue,
                inputAttributes);

            var tb = new TagBuilder("span");
            tb.AddCssClass("btn btn-info input-file-button");
            
            var sb = new StringBuilder();
            sb.Append("<i class=\"icon-upload icon-white\"></i>")
                .AppendFormat("<span>{0}</span>", HttpUtility.HtmlEncode(isMultiple 
                    ? ControlsResources.MultipleUploadButtonTitle : ControlsResources.UploadButtonTitle))
                .Append(input.ToHtmlString());
            
            tb.InnerHtml = sb.ToString();

            var control = new TagBuilder("div");
            control.AddCssClass("input-file");
            control.InnerHtml = tb.ToString() + "<div class=\"input-file-info\"></div>";
            return MvcHtmlString.Create (control.ToString());
        }
        
        
        /// <summary>
        /// Format Credit Card editor
        /// Cards numbers examples: http://www.darkcoding.net/credit-card-numbers/
        /// <example>5264504967381667, 4485261232680516, 180058738148485</example>
        /// </summary>
        internal static IHtmlString CreditCardTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "creditcard", null, true, html.ViewContext.UnobtrusiveJavaScriptEnabled));

            var format = @"<div class=""input-append"">{0}<span class=""add-on""><i class=""icon-credit-card""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, box.ToHtmlString()));
        }

        /// <summary>
        /// Format URL editor
        /// </summary>
        internal static IHtmlString UrlTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "url", "text", true, html.ViewContext.UnobtrusiveJavaScriptEnabled));

            var format = @"<div class=""input-append"">{0}<span class=""add-on""><i class=""icon-globe""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, box.ToHtmlString()));
        }
        
        /// <summary>
        /// Format Phone Number editor
        /// </summary>
        internal static IHtmlString PhoneNumberTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "phoneNumber", "text", true, html.ViewContext.UnobtrusiveJavaScriptEnabled));

            var format = @"<div class=""input-append"">{0}<span class=""add-on""><i class=""icon-phone""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, box.ToHtmlString()));
        }

        /// <summary>
        /// Format E-Mail editor
        /// </summary>
        internal static IHtmlString EmailAddressTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "email", "text", true, html.ViewContext.UnobtrusiveJavaScriptEnabled));

            var format = @"<div class=""input-append"">{0}<span class=""add-on""><i class=""icon-e-mail""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, box.ToHtmlString()));
        }

        /// <summary>
        /// Date editor, got from: https://github.com/eternicode/bootstrap-datepicker
        /// </summary>
        internal static IHtmlString DateTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
              html,
              htmlFieldName,
              formattedValue,
              MergeAttributes(editorAttributes, "date", "text", true));

            var format = @"<div class=""input-append datepicker-control"" data-date-format=""{0}"" >{1}<span class=""add-on""><i class=""icon-calendar""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, GetDateFormat(), box.ToHtmlString()));
        }

        /// <summary>
        /// Time editor
        /// </summary>
        internal static IHtmlString TimeTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
              html,
              htmlFieldName,
              formattedValue,
              MergeAttributes(editorAttributes, "time", "text", true));

            var format = @"<div class=""input-append timepicker-control"" data-time-format=""{0}"" >{1}<span class=""add-on""><i class=""icon-time""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, GetTimeFormat(), box.ToHtmlString()));
        }

        internal static IHtmlString DateTimeTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            var hidden = InputExtensions.Hidden(html, htmlFieldName, formattedValue);
            string dateValue = null, timeValue = null;
            if (formattedValue is DateTime)
            {
                var dt = (DateTime)formattedValue;
                dateValue = dt.ToShortDateString();
                timeValue = dt.ToShortTimeString();
            }

            var date = DateTemplate(html, dateValue, htmlFieldName + DateControlPostfix, metadata, new Dictionary<string, object>(editorAttributes));
            var time = TimeTemplate(html, timeValue, htmlFieldName + TimeControlPostfix, metadata, new Dictionary<string, object>(editorAttributes));

            var format = @"<div class=""datetime"">{0} {1} {2}</div>";
            return MvcHtmlString.Create(String.Format(format,
                date.ToHtmlString(), 
                time.ToHtmlString(),
                hidden.ToHtmlString()));
        }
               

        /// <summary>
        /// Format Postal Code editor
        /// </summary>
        internal static IHtmlString PostalCodeTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            var box = InputExtensions.TextBox(
                html,
                htmlFieldName,
                formattedValue,
                MergeAttributes(editorAttributes, "postal-code", "text", true));
            var format = @"<div class=""input-append"">{0}<span class=""add-on""><i class=""icon-envelope""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, box.ToHtmlString()));
        }
        
        /// <summary>
        /// Format editor for unsigned integer number
        /// </summary>
        internal static IHtmlString UnsignedIntegerTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName, 
            ModelMetadata metadata, 
            IDictionary<string, object> editorAttributes)
        {
            return InputExtensions.TextBox(
                html, 
                htmlFieldName, 
                formattedValue,
                MergeAttributes(editorAttributes, "unsigned_int", "number"));
        }

        /// <summary>
        /// Format editor for signed integer number
        /// </summary>
        internal static IHtmlString SignedIntegerTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            return InputExtensions.TextBox(
                html,
                htmlFieldName,
                formattedValue,
                MergeAttributes(editorAttributes, "signed_int", "number"));
        }

        /// <summary>
        /// Format editor for float number
        /// </summary>
        internal static IHtmlString FloatTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            return InputExtensions.TextBox(
                html,
                htmlFieldName,
                formattedValue,
                MergeAttributes(editorAttributes, "number", "text"));
        }

        /// <summary>
        /// Format editor for boolean value
        /// </summary>
        internal static IHtmlString BooleanTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            var isChecked = false;
            if (formattedValue is bool)
                isChecked = (bool)formattedValue;
            
            return InputExtensions.CheckBox(
                html,
                htmlFieldName,
                isChecked,
                editorAttributes);
        }
        

        private static string GetDateFormat()
        {
            var pattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

            if(String.IsNullOrEmpty(pattern))
                return DefaultDateFormat;

            return pattern;
        }

        private static string GetTimeFormat()
        {
            var pattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;

            if (String.IsNullOrEmpty(pattern))
                return DefaultTimeFormat;

            return pattern;
        }
        
        private static bool ShouldShow(ModelMetadata metadata, TemplateInfo templateInfo)
        {
            if (metadata.ShowForEdit
                    && metadata.ModelType != typeof(System.Data.EntityState) // do not show internal Entity State
                    && (metadata.DataTypeName == "Upload"
                            || metadata.TemplateHint == "Upload" 
                            || !metadata.IsComplexType))
            {
                return !templateInfo.Visited(metadata);
            }
            else
            {
                return false;
            }
        }

        private static IDictionary<string, object> MergeAttributes(
            IDictionary<string, object> editorAttributes, 
            string className, 
            string inputType = null, 
            bool replaceClass = false,
            bool validate = false)
        {
            if (editorAttributes == null)
                editorAttributes = new Dictionary<string, object>();

            if (!String.IsNullOrWhiteSpace(className))
            {
                if (editorAttributes.ContainsKey("class"))
                {
                    if (replaceClass)
                    {
                        editorAttributes["class"] = className;
                    }
                    else
                    {
                        var existingClass = (editorAttributes["class"] as string) ?? String.Empty;
                        var parts = className.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var css in parts)
                        {
                            if (!existingClass.HasCssClass(css))
                                existingClass += " " + css;
                        }

                        editorAttributes["class"] = existingClass;
                    }
                }
                else
                {
                    editorAttributes.Add("class", className);
                }
            }

            if (!String.IsNullOrWhiteSpace(inputType) && !editorAttributes.ContainsKey("type"))
            {
                editorAttributes.Add("type", inputType);
            }

            if (validate)
                editorAttributes.Set("data-val", "true");

            return editorAttributes;
        }

        private static void MergeCurrencyAttributes(IDictionary<string, object> attributes)
        {
            Throw.IfNullArgument(attributes, "attributes");
            
            var numberFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            attributes.Set(DataCarcassValidation, "true");
            attributes.Set(DataNumberDecimalSeparator, numberFormat.CurrencyDecimalSeparator);
            attributes.Set(DataNumberGroupSeparator, numberFormat.CurrencyGroupSeparator);
        }

        private static void LoadMaxLength(HtmlHelper html, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            Throw.IfNullArgument(editorAttributes, "editorAttributes");

            var maxlength = -1;
            var validators = metadata.GetValidators(html.ViewContext.Controller.ControllerContext);
            var lengthLimits = validators.OfType<StringLengthAttributeAdapter>();
            foreach (var limit in lengthLimits)
            {
                foreach (var rule in limit.GetClientValidationRules())
                {
                    var max = rule.ValidationParameters.Get("max", -1);
                    if (max != -1)
                    {
                        if (maxlength == -1 || max < maxlength)
                            maxlength = max;
                    }
                }
            }

            if (maxlength != -1 && !editorAttributes.ContainsKey("maxlength"))
                editorAttributes.Add("maxlength", maxlength);
        }
    }
}
