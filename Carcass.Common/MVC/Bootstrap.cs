using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Carcass.Common.MVC
{
    public static class Bootstrap
    {
        private static object _initLock = new object();
        private static bool _inited = false;

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
