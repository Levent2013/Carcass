using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using log4net;
using MvcExtensions;

using Carcass.Common.Reflection;
using Carcass.Common.Resources;
using Carcass.Common.Utility;

namespace Carcass.Common.MVC
{
    public static class FluentMetadataHelper
    {
        public static IDictionary<string, ModelMetadataItem> RegisterExtendedModelMetadata(
            Type modelType,
            ExtendedModelMetadataProvider metadataProvider,
            IModelMetadataRegistry extMetadataRegistry)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(
                () => Activator.CreateInstance(modelType), 
                modelType);

            var extModelMetadataItem = CreateModelMetadataItem(modelMetadata, modelType.GetCustomAttributes());
            var extModelMetadataProperties = new Dictionary<string, ModelMetadataItem>();
            var modelTypeProperties = modelType.GetProperties();
            foreach (var property in modelMetadata.Properties)
            {
                var modelProperty = modelTypeProperties.Single(p => p.Name == property.PropertyName);
                extModelMetadataProperties.Add(property.PropertyName, CreateModelMetadataItem(property, modelProperty.GetCustomAttributes()));
            }

            extMetadataRegistry.RegisterModel(modelType, extModelMetadataItem);
            extMetadataRegistry.RegisterModelProperties(modelType, extModelMetadataProperties);

            return extMetadataRegistry.GetModelPropertiesMetadata(modelType);
        }

        public static ModelMetadataItem CreateModelMetadataItem(ModelMetadata modelMetadata, IEnumerable<Attribute> attributes)
        {
            Throw.IfNullArgument(modelMetadata, "modelMetadata");
            var item = new ModelMetadataItem();
            
            item.ShowForDisplay = modelMetadata.ShowForDisplay;
            item.IsReadOnly = modelMetadata.IsReadOnly;
            item.ShowForEdit = modelMetadata.ShowForEdit && !modelMetadata.IsReadOnly;
            DisplayAttribute displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            RequiredAttribute requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            DisplayFormatAttribute displayFormatAttribute = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            
            if (displayAttribute != null)
                item.DisplayName = () => displayAttribute.GetName();
            else
                item.DisplayName = () => modelMetadata.GetDisplayName();
            
            if (displayAttribute != null)
                item.ShortDisplayName = () => displayAttribute.GetShortName();
            else
                item.ShortDisplayName = () => modelMetadata.ShortDisplayName;
            
            item.TemplateName = modelMetadata.TemplateHint ?? modelMetadata.DataTypeName;

            if (displayAttribute != null)
                item.Description = () => displayAttribute.GetDescription();
            else
                item.Description = () => modelMetadata.Description;
            
            if (modelMetadata.NullDisplayText != null)
                item.NullDisplayText = () => modelMetadata.NullDisplayText;
            
            if (modelMetadata.Watermark != null)
                item.Watermark = () => modelMetadata.Watermark;
            
            item.HideSurroundingHtml = modelMetadata.HideSurroundingHtml;
            item.RequestValidationEnabled = modelMetadata.RequestValidationEnabled;
            item.IsRequired = modelMetadata.IsRequired;
            item.Order = modelMetadata.Order;
            item.ConvertEmptyStringToNull = modelMetadata.ConvertEmptyStringToNull;

            if (displayFormatAttribute != null)
                item.DisplayFormat = () => displayFormatAttribute.DataFormatString;
            else if (modelMetadata.DisplayFormatString != null)
                item.DisplayFormat = () => modelMetadata.DisplayFormatString;

            if (displayFormatAttribute != null && displayFormatAttribute.ApplyFormatInEditMode)
                item.EditFormat = () => modelMetadata.DisplayFormatString;
            else if (modelMetadata.EditFormatString != null)
                item.EditFormat = () => modelMetadata.EditFormatString;

            item.ApplyFormatInEditMode = item.EditFormat != null;

            return item;
        }
    }
}
