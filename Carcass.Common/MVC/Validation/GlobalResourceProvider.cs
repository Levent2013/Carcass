using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Web.Compilation;

using Carcass.Common.Utility;

namespace Carcass.Common.MVC.Validation
{
    public class GlobalResourceProvider : IResourceProvider
    {
        public GlobalResourceProvider(string classKey)
        {
            Throw.IfBadArgument(() => String.IsNullOrEmpty(classKey), "classKey");
            
            var type = Type.GetType(classKey, false);
            if (type == null)
            {
                var asmName = classKey;
                var className = classKey;
                while(asmName.IndexOf(".") > -1 && type == null) 
                {
                    asmName = asmName.Substring (0, asmName.LastIndexOf("."));
                    className = classKey.Substring(asmName.Length + 1);
                    type = Type.GetType(classKey + "," + asmName, false);
                }
            }

            Throw.IfNullArgument(type, "type");

            Manager = CreateResourceManager(classKey, type.Assembly);
        }
        
        public ResourceManager Manager { get; set; }

        #region IResourceProvider implementation

        public IResourceReader ResourceReader { get; set; }

        public object GetObject(string resourceKey, CultureInfo culture)
        {
            return Manager.GetObject(resourceKey, culture);
        }

        #endregion

        private ResourceManager CreateResourceManager(string classKey, Assembly assembly)
        {
            return new ResourceManager(classKey, assembly);
        }
    }
}
