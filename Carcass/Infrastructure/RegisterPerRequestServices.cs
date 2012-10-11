using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Carcass.Data;
using Carcass.Common.Utility;
using MvcExtensions;

namespace Carcass.Infrastructure
{
    public class RegisterPerRequestServices : BootstrapperTask
    {
        public override TaskContinuation Execute()
        {
            Throw.IfNullArgument(Adapter, "adapter");
            Adapter.RegisterAsPerRequest<DatabaseContext>();
            
            return TaskContinuation.Continue;
        }

        public ContainerAdapter Adapter { get; set; }
    }
}