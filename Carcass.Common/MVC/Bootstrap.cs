using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using log4net;
using MvcExtensions;

using Carcass.Common.Reflection;
using Carcass.Common.Resources;
using Carcass.Common.Utility;

namespace Carcass.Common.MVC
{
    public static class Bootstrap
    {
        private static object _initLock = new object();
        private static bool _inited = false;
        private static ILog _log = LogManager.GetLogger("Carcass.Bootstrap");

        public static void Init()
        {
            lock (_initLock)
            {
                if (_inited)
                    throw new ApplicationException("Carcass Bootstrap already initialized");

                _log.Debug("Init()");

                InitializeViewEngines();
                InitializeModelBinders();

                _inited = true;
            }
        }

        /// <summary>
        /// Disable ASP.NET Web-Forms engine to use only System.Web.Mvc.RazorViewEngine
        /// </summary>
        private static void InitializeViewEngines()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new LocalizedRazorViewEngine());
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

        public static void LocalizeModels(Assembly assembly, string modelsNamespace)
        {
            Throw.IfNullArgument(assembly, "assembly");

            var scanner = new ClassScanner(assembly);
            scanner.Process(modelsNamespace, LocalizeModel);
        }

        public static void LocalizeModel(Type model)
        {
            Throw.IfNullArgument(model, "model");
            
            var metadataRegistry = DependencyResolver.Current.GetService<IModelMetadataRegistry>();
            if(metadataRegistry == null)
            {
                _log.Error("IModelMetadataRegistry service loading failed, MvcExtensions have not been initilized correctly");
                return;
            }
            
            var validationResourcesType = typeof(ValidationResources);
            var props = model.GetProperties();
            _log.DebugFormat("Localize model {0}", model.Name);
            
            var propertiesMetadata = new Dictionary<string, ModelMetadataItem>();
            foreach (var property in props)
            {
                var metadataItem = metadataRegistry.GetModelPropertyMetadata(model, property.Name)
                                    ?? new ModelMetadataItem();
                var required = property.GetCustomAttribute<RequiredAttribute>();
                var dataType = property.GetCustomAttribute<DataTypeAttribute>();
                
                // check that message is not localized yet
                if (required != null && required.ErrorMessageResourceType == null)
                {
                    var validation = metadataItem.GetValidationOrCreateNew<RequiredValidationMetadata>();
                    validation.ErrorMessage = () => ValidationResources.Requred;
                    validation.ErrorMessageResourceType = validationResourcesType;
                    validation.ErrorMessageResourceName = ValidationResourcesLoader.RuleRequired;
                }

                propertiesMetadata.Add(property.Name, metadataItem);
            }

            metadataRegistry.RegisterModelProperties(model, propertiesMetadata);
        }
    }
}
