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
    public static class Localizer
    {
        private static ILog _log = LogManager.GetLogger("Carcass.Bootstrap");
        private static int _convertedModelsCount = 0;

        public static void LocalizeValidators(Assembly assembly, string modelsNamespace)
        {
            Throw.IfNullArgument(assembly, "assembly");

            var extMetadataRegistry = DependencyResolver.Current.GetService<IModelMetadataRegistry>();
            if (extMetadataRegistry == null)
            {
                _log.Error("IModelMetadataRegistry service loading failed, MvcExtensions have not been initilized correctly");
                return;
            }

            var scanner = new ClassScanner(assembly, p => p.GetInterface("IModelMetadataConfiguration") == null);
            scanner.Process(modelsNamespace, model => LocalizeModelValidators(model, extMetadataRegistry));
        }

        private static void LocalizeModelValidators(
            Type modelType,
            IModelMetadataRegistry extMetadataRegistry)
        {
            Throw.IfNullArgument(modelType, "model");

            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => Activator.CreateInstance(modelType), modelType);
            var extendedMetadata = extMetadataRegistry.GetModelPropertiesMetadata(modelType);
            if (extendedMetadata == null)
            {
                _log.DebugFormat("Init fluent configuration for {0}", modelType.Name);
                var metadataProvider = ModelMetadataProviders.Current as ExtendedModelMetadataProvider;

                _convertedModelsCount++;
                extendedMetadata = FluentMetadataHelper.RegisterExtendedModelMetadata(modelType, metadataProvider, extMetadataRegistry);
            }

            _log.DebugFormat("Localize model {0}", modelType.Name);

            var modelTypeProperties = modelType.GetProperties();

            foreach (var property in extendedMetadata)
            {
                var propName = property.Key;
                var modelProperty = modelTypeProperties.Single(p => p.Name == propName);
                var propertyMetadata = metadata.Properties.Single(p => p.PropertyName == propName);
                var propTypeName = modelProperty.PropertyType.Name;

                if (Nullable.GetUnderlyingType(modelProperty.PropertyType) != null 
                        && modelProperty.PropertyType.GenericTypeArguments.Length > 0)
                    propTypeName = modelProperty.PropertyType.GenericTypeArguments[0].Name;
                
                // TODO: Move these checks to separate files
                if (property.Value.IsRequired ?? false)
                {
                    var validation = property.Value.GetValidationOrCreateNew<RequiredValidationMetadata>();
                    // check that message is not localized yet
                    if (validation.ErrorMessage == null && validation.ErrorMessageResourceType == null)
                    {
                        validation.ErrorMessage = () => ValidationResources.Requred;
                    }
                }

                if (propTypeName == "DateTime")
                {
                    // Use CustomValidationMetadata<DataTimeModelValidator> where 
                    // DataTimeModelValidator is ModelValidator
                    // http://msdn.microsoft.com/en-us/library/system.web.mvc.modelvalidator%28v=vs.108%29.aspx
                    // http://stackoverflow.com/questions/3611166/asp-net-mvc-2-problem-with-custom-modelvalidator

                    // var validators = propertyMetadata.GetValidators(new ControllerContext());
                    // Debugger.Launch();
                    // var validation = property.Value.GetValidationOrCreateNew<RequiredValidationMetadata>();
                }
            }
        }
    }
}
