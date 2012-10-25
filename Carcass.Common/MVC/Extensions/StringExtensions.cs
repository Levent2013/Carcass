using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcass.Common.MVC.Extensions
{
    public static class StringExtensions
    {
        public static bool HasCssClass(this string self, string className)
        {
            if (String.IsNullOrWhiteSpace(self))
                return false;

            return (" " + self + " ").IndexOf(" " + className + " ") != -1;
        }
    }
}
