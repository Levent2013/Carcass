using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Web;
using System.Web.Mvc;

namespace Carcass.Common.MVC.Localization
{
    public class LocalizedRazorViewEngine : RazorViewEngine
    {
        private Dictionary<string, string> _localizedViewsCache = new Dictionary<string, string>();
        private object _localizedViewsCacheLock = new object();

        public LocalizedRazorViewEngine()
            : this((IViewPageActivator)null)
        {
        }

        public LocalizedRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
        }
        
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var controllerName = (string)controllerContext.RouteData.Values["controller"];
            var areaName = AreaHelper.GetAreaName(controllerContext.RouteData);
            
            viewName = FindLocalization(areaName, controllerName, viewName);

        
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        private string FindLocalization(string areaName, string controllerName, string viewName)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            var cacheKey = String.Format("{0}/{1}/{2}.{3}", areaName, controllerName, viewName, culture.TwoLetterISOLanguageName);
            lock(_localizedViewsCacheLock)
            {
                if(_localizedViewsCache.ContainsKey(cacheKey))
                    return _localizedViewsCache[cacheKey];
            }

            /*
             [RazorViewEngine sources]
              
             AreaViewLocationFormats = new string[4]
             {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
             };
     
             ViewLocationFormats = new string[4]
             {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
             };
            */

            var localizedViewNames = new[] 
                {
                    // test by full culture name
                    String.Format("{0}.{1}", viewName, culture.Name),
                    // test by culture language name
                    String.Format("{0}.{1}", viewName, culture.TwoLetterISOLanguageName),
                };

            string foundViewName = null;
            foreach (var localizedViewName in localizedViewNames)
            {
                if (!String.IsNullOrEmpty(areaName))
                {
                    foreach (var location in AreaViewLocationFormats)
                    {
                        if (VirtualPathProvider.FileExists(String.Format(location, localizedViewName, controllerName, areaName)))
                        {
                            foundViewName = localizedViewName;
                            break;
                        }
                    }
                }

                foreach (var location in ViewLocationFormats)
                {
                    if (VirtualPathProvider.FileExists(String.Format(location, localizedViewName, controllerName)))
                    {
                        foundViewName = localizedViewName;
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(foundViewName))
                    break;
            }

            if (!String.IsNullOrEmpty(foundViewName))
            {
                viewName = foundViewName;
            }
            
            lock(_localizedViewsCacheLock)
            {
                if(_localizedViewsCache.ContainsKey(cacheKey))
                    _localizedViewsCache[cacheKey] = viewName;
                else
                    _localizedViewsCache.Add(cacheKey, viewName);
            }
            
            return viewName;
        }

    }
}
