using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;

using Newtonsoft.Json;

namespace Carcass.Common.MVC.Html
{
    public static class JavaScriptHelper
    {
        /// <summary>
        /// Write JSON description of server-side enum
        /// </summary>
        /// <typeparam name="T">Base type of enum (used to load value)</typeparam>
        /// <param name="enumType">Type of enum <c>typeof(SomeEnum)</c></param>
        /// <param name="name">The name of enum object in script.</param>
        /// <returns>
        /// JSON string
        /// </returns>
        public static string WriteEnum<T>(Type enumType, string name)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("var {0} = {{ __type: \"enum\"", name);
            
            Array values = Enum.GetValues(enumType);
            foreach (T val in values)
            {
                sb.AppendFormat(NumberFormatInfo.InvariantInfo, ", {0}:{1}", Enum.GetName(enumType, val), val);
            }

            sb.Append("};");
            return sb.ToString();
        }

        public static string WriteResources(Type resourceType, string name)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("if (!window.{0}) window.{0} = {{}};\n", name);

            foreach (var t in resourceType.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendFormat(
                        "window.{0}.{1} = {2};\n", 
                        name,
                        t.Name,
                        EncodeJavaScript(t.GetValue(null, null) as string));
            }

            return sb.ToString();
        }

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

        public static string WriteArray(string arrayName, IEnumerable<object> values)
        {
            if (values == null)
            {
                throw new ArgumentException("values");
            }
            
            var sb = new StringBuilder();
            sb.AppendFormat("var {0} = [ ", arrayName);

            bool first = true;
            foreach (var obj in values)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                
                if (obj is string)
                {
                    sb.Append(EncodeJavaScript(obj as string));
                }
                else if (obj != null)
                {
                    sb.Append(JsonConvert.SerializeObject(obj));
                }
                
                first = false;
            }

            sb.Append("];\r\n");
            return sb.ToString();
        }

        public static string WriteMap(string mapName, IDictionary<string, object> values, bool format = true)
        {
            if (values == null)
            {
                throw new ArgumentException("values");
            }

            var sb = new StringBuilder();
            if (String.IsNullOrEmpty(mapName))
            {
                sb.Append("{");
            }
            else
            {
                sb.AppendFormat("var {0} = {{", mapName);
            }

            bool first = true;
            foreach (var pair in values)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                if (!first)
                {
                    sb.Append(", ");
                }
                
                var value = (pair.Value is string) ? EncodeJavaScript(pair.Value as string) : JsonConvert.SerializeObject(pair.Value);
                sb.AppendFormat("{0}: {1}{2}", EncodeJavaScript(pair.Key), value, format ? "\r\n" : String.Empty);
                first = false;
            }
            
            sb.Append(String.IsNullOrEmpty(mapName) ? "}" : "};\r\n");
            return sb.ToString();
        }
    }
}
