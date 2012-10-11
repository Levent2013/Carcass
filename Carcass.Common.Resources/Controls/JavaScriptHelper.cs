using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace Carcass.Common.Resources
{
    internal static class JavaScriptHelper
    {
        public static string EncodeJavaScript(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "\"\"";
            }

            return "\"" + str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") + "\"";
        }

        public static string EncodeJavaScriptParam(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "''";
            }

            return "'" + str.Replace("<", "&amp;lt;")
                .Replace(">", "&amp;gt;")
                .Replace("\"", "&#34;")
                .Replace("'", "\\'")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r") + "'";

        }
    }
}
