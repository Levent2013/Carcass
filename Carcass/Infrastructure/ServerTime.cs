using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Carcass.Infrastructure
{
    public static class ServerTime
    {
        public static DateTime Now 
        {
            get { return DateTime.UtcNow; }
        }

        public static DateTime ToDisplayDate(DateTime date)
        {
            if (HttpContext.Current == null)
                return date;

            // TODO: Load current user timezone and apply it
            return date;
        }
    }
}