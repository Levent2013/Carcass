using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
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

            _log.Debug("Setup localized Razor view engine");
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

       
    }
}
