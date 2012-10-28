using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Carcass.Common.MVC
{
    public static class AreaHelper
    {
        public static string GetAreaName(RouteBase route)
        {
            IRouteWithArea routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }
            else
            {
                Route routeTmp = route as Route;
                if (routeTmp != null && routeTmp.DataTokens != null)
                    return routeTmp.DataTokens["area"] as string;
                else
                    return (string)null;
            }
        }

        public static string GetAreaName(RouteData routeData)
        {
            object obj;
            if (routeData.DataTokens.TryGetValue("area", out obj))
                return obj as string;
            else
                return AreaHelper.GetAreaName(routeData.Route);
        }
    }
}
