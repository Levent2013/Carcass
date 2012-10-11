using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvcExtensions;

namespace Carcass.Common.Resources.Controls
{
    public class DatepickerResourcesLoader
    {
        private const string DatePickerMessagesFormat =
@"(function($){{
    $.fn.datepicker.dates = $.fn.datepicker.dates || {{}};
    $.fn.datepicker.dates['{0}'] = {{
		days: {1},
		daysShort: {2},
		daysMin: {3},
		months: {4},
		monthsShort: {5},
		today: ""{6}""
	}};
}}(window.jQuery));
";

        public static string GetResources(CultureInfo locale)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("// {0} translation for bootstrap-datepicker\r\n", locale.Name);
            sb.AppendFormat(DatePickerMessagesFormat,
                locale.TwoLetterISOLanguageName,
                DatepickerResourcesLoader.GetDays(locale).ToJson(),
                DatepickerResourcesLoader.GetDaysShort(locale).ToJson(),
                DatepickerResourcesLoader.GetDaysMin(locale).ToJson(),
                DatepickerResourcesLoader.GetMonths(locale).ToJson(),
                DatepickerResourcesLoader.GetMonthsShort(locale).ToJson(),
                DatepickerResourcesLoader.GetToday(locale)
            );

            return sb.ToString();
        }

        private static string[] GetDays(CultureInfo locale)
        {
            return new []
                {
                    DatepickerResources.ResourceManager.GetString("Sunday", locale),
                    DatepickerResources.ResourceManager.GetString("Monday", locale),
                    DatepickerResources.ResourceManager.GetString("Tuesday", locale),
                    DatepickerResources.ResourceManager.GetString("Wednesday", locale),
                    DatepickerResources.ResourceManager.GetString("Thursday", locale),
                    DatepickerResources.ResourceManager.GetString("Friday", locale),
                    DatepickerResources.ResourceManager.GetString("Saturday", locale),
                    DatepickerResources.ResourceManager.GetString("Sunday", locale),
                };
        }

        private static string[] GetDaysShort(CultureInfo locale)
        {
            return new[]
                {
                    DatepickerResources.ResourceManager.GetString("Sun", locale),
                    DatepickerResources.ResourceManager.GetString("Mon", locale),
                    DatepickerResources.ResourceManager.GetString("Tue", locale),
                    DatepickerResources.ResourceManager.GetString("Wed", locale),
                    DatepickerResources.ResourceManager.GetString("Thu", locale),
                    DatepickerResources.ResourceManager.GetString("Fri", locale),
                    DatepickerResources.ResourceManager.GetString("Sat", locale),
                    DatepickerResources.ResourceManager.GetString("Sun", locale),
                };
        }

        private static string[] GetDaysMin(CultureInfo locale)
        {
            return new[]
                {
                    DatepickerResources.ResourceManager.GetString("Su", locale),
                    DatepickerResources.ResourceManager.GetString("Mo", locale),
                    DatepickerResources.ResourceManager.GetString("Tu", locale),
                    DatepickerResources.ResourceManager.GetString("We", locale),
                    DatepickerResources.ResourceManager.GetString("Th", locale),
                    DatepickerResources.ResourceManager.GetString("Fr", locale),
                    DatepickerResources.ResourceManager.GetString("Sa", locale),
                    DatepickerResources.ResourceManager.GetString("Su", locale),
                };
        }

        private static string[] GetMonths(CultureInfo locale)
        {
            return new[]
                {
                    DatepickerResources.ResourceManager.GetString("January", locale),
                    DatepickerResources.ResourceManager.GetString("February", locale),
                    DatepickerResources.ResourceManager.GetString("March", locale),
                    DatepickerResources.ResourceManager.GetString("April", locale),
                    DatepickerResources.ResourceManager.GetString("May", locale),
                    DatepickerResources.ResourceManager.GetString("June", locale),
                    DatepickerResources.ResourceManager.GetString("July", locale),
                    DatepickerResources.ResourceManager.GetString("August", locale),
                    DatepickerResources.ResourceManager.GetString("September", locale),
                    DatepickerResources.ResourceManager.GetString("October", locale),
                    DatepickerResources.ResourceManager.GetString("November", locale),
                    DatepickerResources.ResourceManager.GetString("December", locale),
                };
        }

        private static string[] GetMonthsShort(CultureInfo locale)
        {
            return new[]
                {
                    DatepickerResources.ResourceManager.GetString("Jan", locale),
                    DatepickerResources.ResourceManager.GetString("Feb", locale),
                    DatepickerResources.ResourceManager.GetString("Mar", locale),
                    DatepickerResources.ResourceManager.GetString("Apr", locale),
                    DatepickerResources.ResourceManager.GetString("May", locale),
                    DatepickerResources.ResourceManager.GetString("Jun", locale),
                    DatepickerResources.ResourceManager.GetString("Jul", locale),
                    DatepickerResources.ResourceManager.GetString("Aug", locale),
                    DatepickerResources.ResourceManager.GetString("Sep", locale),
                    DatepickerResources.ResourceManager.GetString("Oct", locale),
                    DatepickerResources.ResourceManager.GetString("Nov", locale),
                    DatepickerResources.ResourceManager.GetString("Dec", locale),
                };
        }

        private static string GetToday(CultureInfo locale)
        {
            return DatepickerResources.ResourceManager.GetString("Today", locale);
        }
    }
}
