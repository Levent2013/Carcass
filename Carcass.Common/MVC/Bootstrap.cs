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
    public sealed class Bootstrap
    {
        private object _initLock = new object();

        private bool _inited = false;
      
        public Bootstrap()
        {
        }

        public void Init()
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

        private void InitializeViewEngines()
        {
            var viewEngines = ViewEngines.Engines;
            var webFormsEngine = viewEngines.OfType<WebFormViewEngine>().FirstOrDefault();
            if (webFormsEngine != null)
                viewEngines.Remove(webFormsEngine);
        }

        private void InitializeModelBinders()
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
