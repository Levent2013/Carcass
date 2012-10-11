using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carcass.Common.Resources;

namespace Carcass.Common.Resources.Controls
{
    public class TimepickerResourcesLoader
    {
        private const string TimepickerResourcesLoaderFormat =
@"(function($){{
    if(!$.fn.timepicker.messages)
        return alert('bootstrap-timepicker.js required');
    $.fn.timepicker.messages['{0}'] = {{
		am: {1}, pm: {2}, now: {3}, hour: {4}, minute: {5}
	}};
}}(window.jQuery));
";

        public static string GetResources(CultureInfo locale)
        {
            var sb = new StringBuilder();
            var resources = TimepickerResources.ResourceManager;
            sb.AppendFormat("// {0} translation for bootstrap-timepicker\r\n", locale.Name);
            sb.AppendFormat(TimepickerResourcesLoaderFormat,
                locale.TwoLetterISOLanguageName,
               JavaScriptHelper.EncodeJavaScript(resources.GetString("AM", locale)),
                JavaScriptHelper.EncodeJavaScript(resources.GetString("PM", locale)),
                JavaScriptHelper.EncodeJavaScript(resources.GetString("Now", locale)),
                JavaScriptHelper.EncodeJavaScript(resources.GetString("Hour", locale)),
                JavaScriptHelper.EncodeJavaScript(resources.GetString("Minute", locale))
            );

            return sb.ToString();
        }

    }
}
