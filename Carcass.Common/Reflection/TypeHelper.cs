using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carcass.Common.Utility;

namespace Carcass.Common.Reflection
{
    public static class TypeHelper
    {
        public static string GetUserFriendlyName(Type type)
        {
            Throw.IfNullArgument(type, "type");
            if (IsNullableValueType(type))
            {
                return String.Format("Nullable<{0}>", type.GenericTypeArguments[0]);
            }

            if (type.IsGenericType)
            {
                var typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
                var sb = new StringBuilder();
                sb.Append(typeName).Append(" <");

                var args = type.GenericTypeArguments.Select(p => GetUserFriendlyName(p)).ToArray();
                sb.Append(String.Join(", ", args)).Append(">");
                
                return sb.ToString();
            }

            return type.Name;
        }
        
        public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> predicate = (Func<Type, bool>)(t =>
            {
                if (t.IsGenericType)
                    return t.GetGenericTypeDefinition() == interfaceType;
                else
                    return false;
            });
            if (!predicate(queryType))
                return Enumerable.FirstOrDefault<Type>((IEnumerable<Type>)queryType.GetInterfaces(), predicate);
            else
                return queryType;
        }

        public static object GetDefaultValue(Type type)
        {
            if (!TypeHelper.TypeAllowsNullValue(type))
                return Activator.CreateInstance(type);
            else
                return (object)null;
        }

        public static bool IsCompatibleObject<T>(object value)
        {
            if (!(value is T))
            {
                if (value == null)
                    return TypeAllowsNullValue(typeof(T));
                else
                    return false;
            }
            else
                return true;
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != (Type)null;
        }

        public static bool TypeAllowsNullValue(Type type)
        {
            if (type.IsValueType)
                return IsNullableValueType(type);
            else
                return true;
        }
    }
}
