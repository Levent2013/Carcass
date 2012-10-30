using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using log4net;
using MvcExtensions;

using Carcass.Common.Reflection;
using Carcass.Common.Resources;
using Carcass.Common.Utility;
using Carcass.Common.MVC.Validation;

namespace Carcass.Common.MVC
{
    public static class Bootstrap
    {
        private static object _initLock = new object();
        private static bool _inited = false;
        private static ILog _log = LogManager.GetLogger("Carcass.Bootstrap");
        private static int _convertedModelsCount = 0;

        public static void Init()
        {
            lock (_initLock)
            {
                if (_inited)
                    throw new ApplicationException("Carcass Bootstrap already initialized");

                InitializeViewEngines();
                InitializeModelBinders();

                _inited = true;
            }
        }

        public static void SetupValidators(Assembly assembly, string modelsNamespace)
        {
            Throw.IfNullArgument(assembly, "assembly");

            var extMetadataRegistry = DependencyResolver.Current.GetService<IModelMetadataRegistry>();
            if (extMetadataRegistry == null)
            {
                _log.Error("IModelMetadataRegistry service loading failed, MvcExtensions have not been initilized correctly");
                return;
            }

            // Modify web.config in run-time and setup custom ResourceProviderFactory
            var globalization = WebConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;
            var readonlyField = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            readonlyField.SetValue(globalization, false);
            globalization.ResourceProviderFactoryType = typeof(Carcass.Common.MVC.Validation.ValidationResourceProviderFactory).FullName;

            var resourcesClass = typeof(ValidationResources).FullName; // tried to use AssemblyQualifiedName but have no success
            DefaultModelBinder.ResourceClassKey = resourcesClass;
            ValidationExtensions.ResourceClassKey = resourcesClass;
            ClientDataTypeModelValidatorProvider.ResourceClassKey = resourcesClass;

            var scanner = new ClassScanner(assembly, p => p.GetInterface("IModelMetadataConfiguration") == null);
            scanner.Process(modelsNamespace, model => LocalizeModelValidators(model, extMetadataRegistry));
        }

        /// <summary>
        /// Disable ASP.NET Web-Forms engine and setup localised Razor ViewEngine
        /// </summary>
        private static void InitializeViewEngines()
        {
            ViewEngines.Engines.Clear();

            _log.Debug("Setup localized Razor view engine");
            ViewEngines.Engines.Add(new Localization.LocalizedRazorViewEngine());
        }
        
        private static void InitializeModelBinders()
        {
            ModelBinders.Binders.Add(
                typeof(DataTypes.PostedFile),
                new DataTypes.Binding.PostedFileArrayBinder());
            
            ModelBinders.Binders.Add(
                typeof(DataTypes.PostedFile[]),
                new DataTypes.Binding.PostedFileArrayBinder());
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
                var propertyName = property.Key;
                var modelProperty = modelTypeProperties.Single(p => p.Name == propertyName);
                var propertyMetadata = metadata.Properties.Single(p => p.PropertyName == propertyName);
                var propTypeName = modelProperty.PropertyType.Name;

                if (Nullable.GetUnderlyingType(modelProperty.PropertyType) != null
                        && modelProperty.PropertyType.GenericTypeArguments.Length > 0)
                    propTypeName = modelProperty.PropertyType.GenericTypeArguments[0].Name;

                if (property.Value.IsRequired ?? false)
                {
                    var validator = property.Value.GetValidationOrCreateNew<RequiredValidationMetadata>();
                    
                    // check that message is not localized yet
                    if (validator.ErrorMessage == null && validator.ErrorMessageResourceType == null)
                    {
                        validator.ErrorMessage = () => ValidationResources.PropertyValueRequired;
                    }
                }
              
                var typeValidator = property.Value.GetValidationOrCreateNew<CustomValidationMetadata<DataTypeModelValidator>>();
                typeValidator.Factory = (mdata, context) => new DataTypeModelValidator(mdata, context);
                typeValidator.Configure = v => DataTypeModelValidator.Configure(v);
            }
        }
       
    }
}
