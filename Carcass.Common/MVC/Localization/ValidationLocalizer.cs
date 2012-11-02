using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using log4net;
using MvcExtensions;

using Carcass.Common.Collections.Extensions;
using Carcass.Common.Reflection;
using Carcass.Common.Resources;
using Carcass.Common.Utility;
using Carcass.Common.MVC.Validation;


namespace Carcass.Common.MVC.Localization
{
    public class ValidationLocalizer
    {
        private Type _modelType;
        private IDictionary<string, ModelMetadataItem>  _extendedMetadata;

        public ValidationLocalizer(Type modelType, IDictionary<string, ModelMetadataItem> extendedMetadata)
        {
            Throw.IfNullArgument(modelType, "modelType");
            Throw.IfNullArgument(extendedMetadata, "extendedMetadata");

            _modelType = modelType;
            _extendedMetadata = extendedMetadata;
        }

        public void ProcessModel()
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => Activator.CreateInstance(_modelType), _modelType);
            var modelTypeProperties = _modelType.GetProperties();
                        
            foreach (var property in _extendedMetadata)
            {
                var propertyName = property.Key;
                var modelProperty = modelTypeProperties.Single(p => p.Name == propertyName);
                var propertyMetadata = metadata.Properties.Single(p => p.PropertyName == propertyName);
                var propTypeName = modelProperty.PropertyType.Name;
                if (Nullable.GetUnderlyingType(modelProperty.PropertyType) != null
                        && modelProperty.PropertyType.GenericTypeArguments.Length > 0)
                {
                    propTypeName = modelProperty.PropertyType.GenericTypeArguments[0].Name;
                }

                var attributes = modelProperty.GetCustomAttributes(false);

                if (propertyMetadata.IsRequired)
                    SetupRequiredValidator(property.Value);
                
                var rangeAttribute = attributes.OfType<RangeAttribute>().FirstOrDefault();
                var extRangeAttribute = FindRangeMetadata(property.Value);
                if (rangeAttribute != null)
                {
                    extRangeAttribute = CreateRangeValidator(property.Value, rangeAttribute);
                }
                
                // check that message is not localized yet
                if (extRangeAttribute != null && extRangeAttribute.ErrorMessage == null && extRangeAttribute.ErrorMessageResourceType == null)
                {
                    extRangeAttribute.ErrorMessage = () => ValidationResources.Range;
                }



                SetupDataTypeValidator(property.Value);
            }
        }

        private ModelValidationMetadata FindRangeMetadata(ModelMetadataItem property)
        {
            return (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<int>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<sbyte>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<short>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<long>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<uint>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<byte>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<ushort>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<ulong>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<float>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<double>>()
                ?? (ModelValidationMetadata)property.GetValidation<RangeValidationMetadata<decimal>>();

        }

        private static ModelValidationMetadata CreateRangeValidator(ModelMetadataItem property, RangeAttribute rangeAttribute)
        {
            object min = rangeAttribute.Minimum;
            object max = rangeAttribute.Maximum;

            // Check all numeric types, no other way found
            ModelValidationMetadata extValidator = null;
            if (min is int)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<int>>();
            else if (min is sbyte)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<sbyte>>();
            else if (min is short)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<short>>();
            else if (min is long)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<long>>();
            else if (min is uint)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<uint>>();
            else if (min is byte)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<byte>>();
            else if (min is ushort)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<ushort>>();
            else if (min is ulong)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<ulong>>();
            else if (min is float)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<float>>();
            else if (min is double)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<double>>();
            else if (min is decimal)
                extValidator = property.GetValidationOrCreateNew<RangeValidationMetadata<decimal>>();

            if (extValidator != null)
            {
                if (rangeAttribute.ErrorMessageResourceType != null)
                {
                    extValidator.ErrorMessageResourceType = rangeAttribute.ErrorMessageResourceType;
                    extValidator.ErrorMessageResourceName = rangeAttribute.ErrorMessageResourceName;
                }
            }

            return extValidator;
        }

        private static void SetupRequiredValidator(ModelMetadataItem property)
        {
            var requiredValidator = property.GetValidationOrCreateNew<RequiredValidationMetadata>();
            
            // check that message is not localized yet
            if (requiredValidator.ErrorMessage == null && requiredValidator.ErrorMessageResourceType == null)
            {
                requiredValidator.ErrorMessage = () => ValidationResources.PropertyValueRequired;
            }
        }

        private static void SetupDataTypeValidator(ModelMetadataItem property)
        {
            var typeValidator = property.GetValidationOrCreateNew<CustomValidationMetadata<DataTypeModelValidator>>();
            typeValidator.Factory = (metadata, context) => new DataTypeModelValidator(metadata, context);
            typeValidator.Configure = v => DataTypeModelValidator.Configure(v);
        }
    }
}
