using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Carcass.Common.Utility;

namespace Carcass.Common.MVC
{
    [Serializable]
    public class ApplicationContext
    {
        private static ApplicationContext _emptyContext = new ApplicationContext();

        public const string ContextKey = "Carcass-ApplicationContext";

        public static ApplicationContext Current
        {
            get 
            { 
                Throw.IfNullArgument(HttpContext.Current, "HttpContext.Current");
                return (HttpContext.Current.Items[ContextKey] as ApplicationContext) ?? _emptyContext;
            }

            set 
            { 
                Throw.IfNullArgument(HttpContext.Current, "HttpContext.Current");
                HttpContext.Current.Items[ContextKey] = value;
            }
        }


        public TimeSpan TimeZoneOffset { get; set; }
    }
}
