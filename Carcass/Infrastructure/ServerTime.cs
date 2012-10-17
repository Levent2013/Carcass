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
            // TODO: Replace with DateTime.UtcNow, add timezone for each user, 
            //       and update all datetime rendering 
            get { return DateTime.Now; }
        }
    }
}