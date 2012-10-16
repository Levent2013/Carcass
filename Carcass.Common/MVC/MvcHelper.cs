using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Carcass.Common.MVC
{
    public static class MvcHelper
    {
        public static string GetHtmlPreview(string content, int symbolsCount = 255)
        {
            if (String.IsNullOrEmpty(content))
                return content;
            
            var regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", RegexOptions.Singleline);
            return regex.Replace(content, String.Empty);
        }
    }
}
