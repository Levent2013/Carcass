using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace Carcass.Common.MVC.Validation
{
    public sealed class ValidationResourceProviderFactory: ResourceProviderFactory
    {
        public ValidationResourceProviderFactory()
        {
        }

        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            return new GlobalResourceProvider(classKey);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            throw new NotImplementedException("Local resources are not supported yet");
        }
    }
}
