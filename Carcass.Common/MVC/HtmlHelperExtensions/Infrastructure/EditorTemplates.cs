using System;
using System.Collections;
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
using Carcass.Common.Reflection;
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
            // Complex models
            { "Object", ObjectTemplate },
            { "Collection", CollectionTemplate },
            
            { "RenderAction", RenderActionTemplate },

            // Simple models
            { "HiddenInput", HiddenInputTemplate },
            { "MultilineText", MultilineTextTemplate },
            { "Password", PasswordTemplate },
            { "Text", StringTemplate },
            { "CreditCard", CreditCardTemplate },
            { "Currency", CurrencyTemplate },
            { "Html", HtmlTemplate },
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
        };

        internal static ActionDelegate FindAction(string fieldType)
        {
            return _defaultEditorActions.ContainsKey(fieldType) 
                ? _defaultEditorActions[fieldType]
                : NotImplementedTemplate; 
        }

        internal static IHtmlString FormatAlert(string format, params object[] args)
        {
            var content = String.Format(format, args);
            return MvcHtmlString.Create(String.Format("<div class=\"alert\">{0}</div>", content));
        }

        internal static IHtmlString NotImplementedTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata modelMetadata,
            IDictionary<string, object> htmlAttributes)
        {
            var propertyName = htmlFieldName ?? modelMetadata.DisplayName ?? modelMetadata.PropertyName;

            if(!String.IsNullOrEmpty(propertyName))
            {
                return FormatAlert("Editor for property \"{0}\" is not implemented", propertyName);
            }

            return FormatAlert("Editor for model of type \"{0}\" is not implemented");
        }

        internal static IHtmlString CollectionTemplate(HtmlHelper html, 
            object formattedValue, 
            string htmlFieldName,
            ModelMetadata modelMetadata, 
            IDictionary<string, object> htmlAttributes)
        {
            var viewData = html.ViewContext.ViewData;
            var templateInfo = viewData.TemplateInfo;
            object model = modelMetadata.Model;
            if (model == null)
            {
                return MvcHtmlString.Empty;
            }

            IEnumerable enumerable = model as IEnumerable;
            if (enumerable == null)
            {
                throw new InvalidOperationException(
                        String.Format("Type {0} must implement System.Collections.IEnumerable", model.GetType().FullName));
            }

            var itemType = typeof(string);
            var genericInterface = TypeHelper.ExtractGenericInterface(enumerable.GetType(), typeof(IEnumerable<>));
            if (genericInterface != null)
                itemType = genericInterface.GetGenericArguments()[0];

            bool isItemTypeNullable = TypeHelper.IsNullableValueType(itemType);
            
            var stringBuilder = new StringBuilder();
            int num = 0;
            var htmlFieldNamePrefix = templateInfo.GetFullHtmlFieldName(htmlFieldName);
            if (!String.IsNullOrEmpty(htmlFieldNamePrefix))
                htmlFieldNamePrefix += ".";

            IEnumerator enumerator = enumerable.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (num > 0)
                        stringBuilder.Append("<hr />");
                    
                    object item = enumerator.Current;
                    var modelType = itemType;
                    if (item != null && !isItemTypeNullable)
                        modelType = item.GetType();

                    var metadataForType = ModelMetadataProviders.Current.GetMetadataForType(() => item, modelType);
                    var itemFieldName = String.Format(
                        (IFormatProvider) CultureInfo.InvariantCulture, 
                        "{0}[{1}]", 
                        htmlFieldNamePrefix,
                        num++
                    );

                    var editor = html.RenderCarcassEditor(metadataForType, null, itemFieldName);
                    stringBuilder.Append(editor.ToHtmlString());
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
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
            
            var inlineValidation = htmlAttributes.Get("InlineValidation", true);
            var validationClass = htmlAttributes.Get("ValidationClass", CarcassMvcSettings.ValidationMessageClass);
            var validationAttrs = new Dictionary<string, object>() { { "class", validationClass } };

            var editorAttributes = new Dictionary<string, object>();
            if (htmlAttributes.ContainsKey("EditorClass"))
                editorAttributes.Add("class", htmlAttributes["EditorClass"]);

            var content = new TagBuilder("div");
            if (htmlAttributes.ContainsKey("CssClass"))
                content.AddCssClass(htmlAttributes["CssClass"] as string);
                
            sb.Append(content.ToString(TagRenderMode.StartTag));

            // setup controls hierarchy
            var initHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
            if (!String.IsNullOrEmpty(htmlFieldName))
                templateInfo.HtmlFieldPrefix = templateInfo.GetFullHtmlFieldName(htmlFieldName);

            try
            {
                foreach (var metadata in modelMetadata.Properties.Where(pm => EditorTemplates.ShouldShow(pm, templateInfo)))
                {
                    var fieldType = metadata.TemplateHint ?? metadata.DataTypeName;
                    var isHidden = fieldType == "HiddenInput";
                    var itemFieldName = metadata.PropertyName;

                    if (!isHidden && !metadata.HideSurroundingHtml)
                    {
                        var labelAttributes = isHorisontalForm
                            ? new Dictionary<string, object> { { "class", CarcassMvcSettings.BootsrapFormLabelClass } } : null;
                        var label = LabelExtensions.FormatCarcassLabel(html, metadata, itemFieldName, null, labelAttributes);

                        if (isHorisontalForm)
                            sb.AppendFormat("<div class=\"{0}\">", CarcassMvcSettings.BootsrapFormFieldClass);
                        sb.Append(label.ToHtmlString());
                        if (isHorisontalForm)
                            sb.AppendFormat("<div class=\"{0}\">", CarcassMvcSettings.BootsrapFormFieldControlsClass);
                    }

                    // Reset ViewBag values
                    if (containerData.ContainsKey(itemFieldName))
                        containerData.Remove(itemFieldName);

                    var controlAttributes = editorAttributes.Clone();

                    var fullHtmlFieldName = templateInfo.GetFullHtmlFieldName(itemFieldName);
                    var validationAttributes = html.GetUnobtrusiveValidationAttributes(itemFieldName, metadata);
                    formContext.RenderedField(fullHtmlFieldName, false);

                    foreach (var attr in validationAttributes) 
                    {
                        if(!controlAttributes.ContainsKey(attr.Key))
                            controlAttributes.Add(attr);
                    }

                    sb.Append("\r\n");
                    sb.Append(html.RenderCarcassEditor(metadata, null, itemFieldName, controlAttributes).ToHtmlString());
                    sb.Append("\r\n");

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
                                    ValidationExtensions.FieldValidationMessage(html, metadata, itemFieldName + DateControlPostfix, null, validationAttrs.Clone())
                                        .ToHtmlString()).Append(" ");
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, itemFieldName + TimeControlPostfix, null, validationAttrs.Clone())
                                        .ToHtmlString()).Append(" ");
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, itemFieldName, null, validationAttrs.Clone())
                                        .ToHtmlString());
                            }
                            else
                            {
                                sb.Append(
                                    ValidationExtensions.FieldValidationMessage(html, metadata, itemFieldName, null, validationAttrs.Clone())
                                    .ToHtmlString());
                            }
                        }

                        if (isHorisontalForm)
                            sb.Append("</div></div>\r\n");
                    }
                }
            }
            finally
            {
                templateInfo.HtmlFieldPrefix = initHtmlFieldPrefix;
            }

            sb.Append(content.ToString(TagRenderMode.EndTag));
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
                LoadAttributes(editorAttributes, "multi-line"));
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
                LoadAttributes(editorAttributes, "html-editor"));
        }

        internal static IHtmlString PasswordTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            // evaluate field value maxlength
            LoadMaxLength(html, metadata, editorAttributes);
            
            return InputExtensions.Password(
                html, 
                htmlFieldName, 
                formattedValue,
                LoadAttributes(editorAttributes, "password", "password"));
        }

        internal static IHtmlString StringTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            // evaluate field value maxlength
            LoadMaxLength(html, metadata, editorAttributes);
            
            return InputExtensions.TextBox(
                html,
                htmlFieldName, 
                formattedValue,
                LoadAttributes(editorAttributes, "single-line"));
        }

        internal static IHtmlString CurrencyTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            editorAttributes = LoadAttributes(editorAttributes, "currency");
            LoadDecimalValidationAttributes (editorAttributes);

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
            var inputAttributes = LoadAttributes(editorAttributes, null, "file");
            
            var property = metadata.ContainerType.GetProperty(
                htmlFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            Throw.IfNullArgument(property, "Property {0} not found in {1}", htmlFieldName, metadata.ContainerType.Name);

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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var attrs = LoadAttributes(editorAttributes, "creditcard", "text", true);
            var box = InputExtensions.TextBox(html, htmlFieldName, formattedValue, attrs);

            var format = @"<div class=""input-append {0}"">{1}<span class=""add-on""><i class=""icon-credit-card""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, box.ToHtmlString()));
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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var attrs = LoadAttributes(editorAttributes, "url", "text", true);
            var box = InputExtensions.TextBox(html, htmlFieldName, formattedValue, attrs);

            var format = @"<div class=""input-append {0}"">{1}<span class=""add-on""><i class=""icon-globe""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, box.ToHtmlString()));
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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var attrs = LoadAttributes(editorAttributes, "phone-number", "text", true);
            var box = InputExtensions.TextBox(html, htmlFieldName, formattedValue, attrs);

            var format = @"<div class=""input-append {0}"">{1}<span class=""add-on""><i class=""icon-phone""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, box.ToHtmlString()));
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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var attrs = LoadAttributes(editorAttributes, "email", "text", true);
            var box = InputExtensions.TextBox(html, htmlFieldName, formattedValue, attrs);

            var format = @"<div class=""input-append {0}"">{1}<span class=""add-on""><i class=""icon-e-mail""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, box.ToHtmlString()));
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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var box = InputExtensions.TextBox(
              html,
              htmlFieldName,
              formattedValue,
              LoadAttributes(editorAttributes, "date", "text", true));

            var format = @"<div class=""input-append {0} datepicker-control"" data-date-format=""{1}"" >{2}<span class=""add-on""><i class=""icon-calendar""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, GetDateFormat(), box.ToHtmlString()));
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
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var box = InputExtensions.TextBox(
              html,
              htmlFieldName,
              formattedValue,
              LoadAttributes(editorAttributes, "time", "text", true));

            var format = @"<div class=""input-append {0} timepicker-control"" data-time-format=""{1}"" >{2}<span class=""add-on""><i class=""icon-time""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, GetTimeFormat(), box.ToHtmlString()));
        }

        internal static IHtmlString DateTimeTemplate(HtmlHelper html,
            object formattedValue,
            string htmlFieldName,
            ModelMetadata metadata,
            IDictionary<string, object> editorAttributes)
        {
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var hidden = InputExtensions.Hidden(html, htmlFieldName, formattedValue);
            string dateValue = null, timeValue = null;
            if (formattedValue is DateTime)
            {
                var dt = (DateTime)formattedValue;
                dateValue = dt.ToShortDateString();
                timeValue = dt.ToShortTimeString();
            }

            var attrs = LoadAttributes(editorAttributes, String.Empty, null, true);
            var date = DateTemplate(html, dateValue, htmlFieldName + DateControlPostfix, metadata, attrs);
            var time = TimeTemplate(html, timeValue, htmlFieldName + TimeControlPostfix, metadata, attrs);

            var format = @"<div class=""datetime {0}"">{1} {2} {3}</div>";
            return MvcHtmlString.Create(String.Format(format,
                oldClass,
                date.ToHtmlString(), 
                time.ToHtmlString(),
                hidden.ToHtmlString()));
        }
               

        /// <summary>
        /// Format Postal Code editor
        /// </summary>
        internal static IHtmlString PostalCodeTemplate(HtmlHelper html, object formattedValue, string htmlFieldName, ModelMetadata metadata, IDictionary<string, object> editorAttributes)
        {
            var oldClass = LoadHtmlAttribute(editorAttributes, "class");
            var attrs = LoadAttributes(editorAttributes, "postal-code", "text", true);
            var box = InputExtensions.TextBox(html, htmlFieldName, formattedValue, attrs);

            var format = @"<div class=""input-append {0}"">{1}<span class=""add-on""><i class=""icon-envelope""></i></span></div>";
            return MvcHtmlString.Create(String.Format(format, oldClass, box.ToHtmlString()));
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
                LoadAttributes(editorAttributes, "unsigned_int", "number"));
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
                LoadAttributes(editorAttributes, "signed_int", "number"));
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
            var attributes = LoadAttributes(editorAttributes, "number", "text");
            LoadDecimalValidationAttributes(editorAttributes);
            return InputExtensions.TextBox(html, htmlFieldName, formattedValue, attributes);
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

            return InputExtensions.CheckBox(html, htmlFieldName, isChecked, LoadAttributes(editorAttributes, "boolean"));
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

        private static string LoadHtmlAttribute(IDictionary<string, object> attributes, string name)
        {
            if (attributes == null)
                return null;

            return attributes.Get<string>(name);
        }


        private static IDictionary<string, object> LoadAttributes(
            IDictionary<string, object> editorAttributes, 
            string className, 
            string inputType = null, 
            bool replaceClass = false)
        {
            if (editorAttributes == null)
                editorAttributes = new Dictionary<string, object>();

            if (className != null) // white-space allowed to reset classes
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

            return editorAttributes;
        }

        private static void LoadDecimalValidationAttributes (IDictionary<string, object> attributes)
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
