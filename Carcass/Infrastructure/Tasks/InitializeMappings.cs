using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AutoMapper;
using MvcExtensions;

namespace Carcass.Infrastructure.Tasks
{
    /// <summary>
    /// Initialize common Automapper mappings here
    /// </summary>
    public class InitializeMappings : BootstrapperTask
    {
        public override TaskContinuation Execute()
        {
            Mapper.CreateMap<int, string>().ConvertUsing(p => p.ToString(CultureInfo.InvariantCulture));


            Mapper.AssertConfigurationIsValid();

            return TaskContinuation.Continue;
        }
    }
}